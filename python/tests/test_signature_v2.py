import asyncio
import base64
import hashlib
import hmac
from types import SimpleNamespace
from unittest.mock import patch
from urllib.parse import quote, quote_plus

import pytest
from Tea.core import TeaCore
from alibabacloud_openapi_util.client import Client as OpenApiUtilClient
from alibabacloud_tea_util.models import RuntimeOptions
from alibabacloud_tea_openapi.client import Client
from alibabacloud_tea_openapi.models import Config, OpenApiRequest, Params


@pytest.mark.parametrize('algorithm,digest', [(None, hashlib.sha256), ('HMAC-SHA1', hashlib.sha1)])
@pytest.mark.parametrize('style', ['RPC', 'ROA'])
@pytest.mark.parametrize('is_async', [False, True])
def test_v2_signatures_and_algorithm_field(algorithm, digest, style, is_async):
    config = Config(access_key_id='id', access_key_secret='secret', endpoint='example.com', signature_algorithm='v2')
    client = Client(config)
    params = Params(action='Action', version='2020-01-01', protocol='HTTPS', pathname='/resource',
                    method='GET', auth_type='AK', style=style, body_type='none')
    captured = []

    def send(request, runtime):
        captured.append(request)
        return SimpleNamespace(status_code=200, headers={})

    async def send_async(request, runtime):
        return send(request, runtime)

    request = OpenApiRequest(headers={'X-Acs-Signature-Method': algorithm} if algorithm else None)
    if is_async:
        with patch.object(TeaCore, 'async_do_action', side_effect=send_async):
            loop = asyncio.new_event_loop()
            try:
                loop.run_until_complete(client.call_api_async(params, request, RuntimeOptions()))
            finally:
                loop.close()
    else:
        with patch.object(TeaCore, 'do_action', side_effect=send):
            client.call_api(params, request, RuntimeOptions())

    request = captured[0]
    expected_method = 'HMAC-SHA1' if algorithm else 'HMAC-SHA256'
    if style == 'RPC':
        assert request.query['SignatureMethod'] == expected_method
        signed_params = {k: v for k, v in request.query.items() if k != 'Signature' and v is not None}
        query = '&'.join(f'{quote(k, safe="~")}={quote(v, safe="~")}' for k, v in sorted(signed_params.items()))
        string_to_sign = f'GET&%2F&{quote_plus(query, safe="~")}'
        expected = base64.b64encode(hmac.new(b'secret&', string_to_sign.encode(), digest).digest()).decode()
        assert request.query['Signature'] == expected
    else:
        assert request.headers['x-acs-signature-method'] == expected_method
        string_to_sign = OpenApiUtilClient.get_string_to_sign(request)
        expected = base64.b64encode(hmac.new(b'secret', string_to_sign.encode(), digest).digest()).decode()
        assert request.headers['authorization'] == 'acs id:' + expected


def test_header_precedence_and_conflicts():
    headers = {'x-acs-signature-method': 'HMAC-SHA256', 'X-Acs-Signature-Method': 'HMAC-SHA1'}
    assert Client._get_v2_signature_method(headers, {'X-Acs-Signature-Method': 'HMAC-SHA1'},
                                           {'x-acs-signature-method': 'HMAC-SHA256'}) == 'HMAC-SHA1'
    assert headers == {'x-acs-signature-method': 'HMAC-SHA1'}
    with pytest.raises(ValueError, match='Conflicting'):
        Client._get_v2_signature_method({}, {'x-acs-signature-method': 'HMAC-SHA1',
                                             'X-Acs-Signature-Method': 'HMAC-SHA256'})
    with pytest.raises(ValueError, match='Unsupported'):
        Client._get_v2_signature_method({}, {'x-acs-signature-method': 'HMAC-MD5'})


def test_rpc_special_characters_and_old_util():
    params = {'你好': 'a b+&=中', 'empty': '', 'skip': None}
    query = '&'.join(f'{quote(k, safe="~")}={quote(v, safe="~")}' for k, v in sorted(params.items()) if v is not None)
    to_sign = f'POST&%2F&{quote_plus(query, safe="~")}'
    expected = base64.b64encode(hmac.new(b'secret&', to_sign.encode(), hashlib.sha256).digest()).decode()
    assert Client._get_v2_rpc_signature(params, 'POST', 'secret', 'HMAC-SHA256') == expected
    assert Client._get_v2_rpc_signature(params, 'POST', 'secret', 'HMAC-SHA1') == OpenApiUtilClient.get_rpcsignature(params, 'POST', 'secret')


@pytest.mark.parametrize('auth_type', ['Anonymous', 'AK'])
def test_roa_non_hmac_auth_keeps_original_header(auth_type):
    config = Config(bearer_token='token', endpoint='example.com', signature_algorithm='v2')
    client = Client(config)
    params = Params(action='Action', version='2020-01-01', protocol='HTTPS', pathname='/',
                    method='GET', auth_type=auth_type, style='ROA', body_type='none')
    captured = []

    def send(request, runtime):
        captured.append(request)
        return SimpleNamespace(status_code=200, headers={})

    with patch.object(TeaCore, 'do_action', side_effect=send):
        client.call_api(params, OpenApiRequest(), RuntimeOptions())
    assert captured[0].headers['x-acs-signature-method'] == 'HMAC-SHA1'
