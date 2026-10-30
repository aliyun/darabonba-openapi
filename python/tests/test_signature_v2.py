import base64
import asyncio
import hashlib
import hmac
from types import SimpleNamespace
from unittest.mock import patch
from urllib.parse import quote, quote_plus

import pytest
from darabonba.core import DaraCore
from darabonba.runtime import RuntimeOptions

from alibabacloud_tea_openapi.client import Client
from alibabacloud_tea_openapi.models import Config, OpenApiRequest, Params
from alibabacloud_tea_openapi.utils import Utils


@pytest.mark.parametrize('algorithm,digest', [(None, hashlib.sha256), ('HMAC-SHA1', hashlib.sha1)])
@pytest.mark.parametrize('style', ['RPC', 'ROA'])
@pytest.mark.parametrize('is_async', [False, True])
def test_v2_signatures_and_algorithm_field(algorithm, digest, style, is_async):
    config = Config(access_key_id='id', access_key_secret='secret', endpoint='example.com',
                    signature_algorithm='v2')
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
        with patch.object(DaraCore, 'async_do_action', side_effect=send_async):
            loop = asyncio.new_event_loop()
            try:
                loop.run_until_complete(client.call_api_async(params, request, RuntimeOptions()))
            finally:
                loop.close()
    else:
        with patch.object(DaraCore, 'do_action', side_effect=send):
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
        string_to_sign = Utils.get_string_to_sign(request)
        expected = base64.b64encode(hmac.new(b'secret', string_to_sign.encode(), digest).digest()).decode()
        assert request.headers['authorization'] == 'acs id:' + expected


def test_roa_explicit_sha1_header_overrides_default():
    config = Config(access_key_id='id', access_key_secret='secret', endpoint='example.com', signature_algorithm='v2')
    client = Client(config)
    params = Params(action='Action', version='2020-01-01', protocol='HTTPS', pathname='/',
                    method='GET', auth_type='AK', style='ROA', body_type='none')
    captured = []

    def send(request, runtime):
        captured.append(request)
        return SimpleNamespace(status_code=200, headers={})

    with patch.object(DaraCore, 'do_action', side_effect=send):
        client.call_api(params, OpenApiRequest(headers={'x-acs-signature-method': 'HMAC-SHA1'}), RuntimeOptions())
    request = captured[0]
    expected = base64.b64encode(hmac.new(b'secret', Utils.get_string_to_sign(request).encode(), hashlib.sha1).digest()).decode()
    assert request.headers['authorization'] == 'acs id:' + expected


def test_legacy_util_keeps_sha1():
    expected = base64.b64encode(hmac.new(b'secret', b'payload', hashlib.sha1).digest()).decode()
    assert Utils.get_roasignature('payload', 'secret') == expected


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


@pytest.mark.parametrize('override,expected', [(None, 'HMAC-SHA1'), ('HMAC-SHA256', 'HMAC-SHA256')])
@pytest.mark.parametrize('is_async', [False, True])
def test_rpc_explicit_sha1_query_and_header_precedence(override, expected, is_async):
    client = Client(Config(access_key_id='id', access_key_secret='secret', endpoint='example.com', signature_algorithm='v2'))
    params = Params(action='Action', version='2020-01-01', protocol='HTTPS', pathname='/',
                    method='GET', auth_type='AK', style='RPC', body_type='none')
    captured = []

    def send(request, runtime):
        captured.append(request)
        return SimpleNamespace(status_code=200, headers={})

    async def send_async(request, runtime):
        return send(request, runtime)

    request = OpenApiRequest(query={'SignatureMethod': 'HMAC-SHA1'},
                             headers={'x-acs-signature-method': override} if override else None)
    if is_async:
        with patch.object(DaraCore, 'async_do_action', side_effect=send_async):
            loop = asyncio.new_event_loop()
            try:
                loop.run_until_complete(client.call_api_async(params, request, RuntimeOptions()))
            finally:
                loop.close()
    else:
        with patch.object(DaraCore, 'do_action', side_effect=send):
            client.call_api(params, request, RuntimeOptions())
    query = captured[0].query
    assert query['SignatureMethod'] == expected
    signed = '&'.join(f'{quote(k, safe="~")}={quote(v, safe="~")}'
                      for k, v in sorted(query.items()) if k != 'Signature' and v is not None)
    digest = hashlib.sha1 if expected == 'HMAC-SHA1' else hashlib.sha256
    expected_signature = base64.b64encode(hmac.new(b'secret&', f'GET&%2F&{quote_plus(signed, safe="~")}'.encode(), digest).digest()).decode()
    assert query['Signature'] == expected_signature


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

    with patch.object(DaraCore, 'do_action', side_effect=send):
        client.call_api(params, OpenApiRequest(), RuntimeOptions())
    assert captured[0].headers['x-acs-signature-method'] == 'HMAC-SHA1'
