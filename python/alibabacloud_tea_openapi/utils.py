# -*- coding: utf-8 -*-
# This file is auto-generated, don't edit it. Thanks.
from __future__ import annotations
from darabonba.model import DaraModel
from typing import Dict, Any, List
import binascii
import datetime
import hashlib
import hmac
import base64
import copy
import platform
import time
import Tea
import threading
import random
import hashlib
from cryptography.hazmat.backends import default_backend
from cryptography.hazmat.primitives import hashes
from cryptography.hazmat.primitives.asymmetric import padding
from cryptography.hazmat.primitives.serialization import load_pem_private_key
from urllib.parse import quote_plus, quote
from darabonba.utils.stream import STREAM_CLASS
from darabonba.utils.form import Form
from darabonba.core import DaraCore
from datetime import datetime
from typing import Any, Dict, List
from .sm3 import hash_sm3, Sm3

_process_start_time = int(time.time() * 1000)
_seqId = 0

def to_str(val):
    if val is None:
        return val

    if isinstance(val, bytes):
        return str(val, encoding='utf-8')
    else:
        return str(val)


def rsa_sign(plaintext, secret):
    if not secret.startswith(b'-----BEGIN RSA PRIVATE KEY-----'):
        secret = b'-----BEGIN RSA PRIVATE KEY-----\n%s' % secret
    if not secret.endswith(b'-----END RSA PRIVATE KEY-----'):
        secret = b'%s\n-----END RSA PRIVATE KEY-----' % secret

    key = load_pem_private_key(secret, password=None, backend=default_backend())
    return key.sign(plaintext, padding.PKCS1v15(), hashes.SHA256())


def signature_method(secret, source, sign_type):
    source = source.encode('utf-8')
    secret = secret.encode('utf-8')
    if sign_type == 'ACS3-HMAC-SHA256':
        return hmac.new(secret, source, hashlib.sha256).digest()
    elif sign_type == 'ACS3-HMAC-SM3':
        return hmac.new(secret, source, Sm3).digest()
    elif sign_type == 'ACS3-RSA-SHA256':
        return rsa_sign(source, secret)


def get_canonical_query_string(query):
    if query is None or len(query) <= 0:
        return ''
    canon_keys = []
    for k, v in query.items():
        if v is not None:
            canon_keys.append(k)

    canon_keys.sort()
    query_string = ''
    for key in canon_keys:
        value = quote(query[key], safe='~', encoding='utf-8')
        if value is None:
            s = f'{key}&'
        else:
            s = f'{key}={value}&'
        query_string += s
    return query_string[:-1]


def get_canonicalized_headers(headers):
    canon_keys = []
    tmp_headers = {}
    for k, v in headers.items():
        if v is not None:
            if k.lower() not in canon_keys:
                canon_keys.append(k.lower())
                tmp_headers[k.lower()] = [to_str(v).strip()]
            else:
                tmp_headers[k.lower()].append(to_str(v).strip())

    canon_keys.sort()
    canonical_headers = ''
    for key in canon_keys:
        header_entry = ','.join(sorted(tmp_headers[key]))
        s = f'{key}:{header_entry}\n'
        canonical_headers += s
    return canonical_headers, ';'.join(canon_keys)


class Utils(object):
    """
    This is for OpenApi Util
    """

    @staticmethod
    def convert(body, content):
        """
        Convert all params of body other than type of readable into content

        @param body: source Model

        @param content: target Model

        @return: void
        """
        body_map = Utils._except_stream(body.to_map())
        content.from_map(body_map)

    @staticmethod
    def _except_stream(val):
        if isinstance(val, dict):
            result = {}
            for k, v in val.items():
                result[k] = Utils._except_stream(v)
            return result
        elif isinstance(val, list):
            result = []
            for i in val:
                if i is not None:
                    item = Utils._except_stream(i)
                    if item is not None:
                        result.append(item)
                else:
                    result.append(Utils._except_stream(i))
            return result
        elif isinstance(val, STREAM_CLASS):
            return None
        return val

    @staticmethod
    def _get_canonicalized_headers(headers):
        canon_keys = []
        for k in headers:
            if k.startswith('x-acs-'):
                canon_keys.append(k)
        canon_keys = sorted(canon_keys)
        canon_header = ''
        for k in canon_keys:
            canon_header += '%s:%s\n' % (k, headers[k])
        return canon_header

    @staticmethod
    def _get_canonicalized_resource(pathname, query):
        if len(query) <= 0:
            return pathname
        resource = '%s?' % pathname
        query_list = sorted(list(query))
        for key in query_list:
            if query[key] is not None:
                if query[key] == '':
                    s = '%s&' % key
                else:
                    s = '%s=%s&' % (key, query[key])
                resource += s
        return resource[:-1]

    @staticmethod
    def get_string_to_sign(request):
        """
        Get the string to be signed according to request

        @param request:  which contains signed messages

        @return: the signed string
        """
        method, pathname, headers, query = request.method, request.pathname, request.headers, request.query

        accept = '' if headers.get('accept') is None else headers.get('accept')
        content_md5 = '' if headers.get('content-md5') is None else headers.get('content-md5')
        content_type = '' if headers.get('content-type') is None else headers.get('content-type')
        date = '' if headers.get('date') is None else headers.get('date')

        header = '%s\n%s\n%s\n%s\n%s\n' % (method, accept, content_md5, content_type, date)
        canon_headers = Utils._get_canonicalized_headers(headers)
        canon_resource = Utils._get_canonicalized_resource(pathname, query)
        sign_str = header + canon_headers + canon_resource
        return sign_str

    @staticmethod
    def get_roasignature(string_to_sign, secret):
        """
        Get signature according to stringToSign, secret

        @type string_to_sign: str
        @param string_to_sign:  the signed string

        @type secret: str
        @param secret: accesskey secret

        @return: the signature
        """
        hash_val = hmac.new(secret.encode('utf-8'), string_to_sign.encode('utf-8'), hashlib.sha1).digest()
        signature = base64.b64encode(hash_val).decode('utf-8')
        return signature

    @staticmethod
    def _object_handler(key, value, out):
        if value is None:
            return

        if isinstance(value, dict):
            for k, v in value.items():
                Utils._object_handler('%s.%s' % (key, k), v, out)
        elif isinstance(value, DaraModel):
            for k, v in value.to_map().items():
                Utils._object_handler('%s.%s' % (key, k), v, out)
        elif isinstance(value, (list, tuple)):
            for index, val in enumerate(value):
                Utils._object_handler('%s.%s' % (key, index + 1), val, out)
        else:
            if key.startswith('.'):
                key = key[1:]
            if isinstance(value, bytes):
                out[key] = str(value, encoding='utf-8')
            elif not isinstance(value, STREAM_CLASS):
                out[key] = str(value)

    @staticmethod
    def anyify_map_value(
        m: Dict[str, str],
    ) -> Dict[str, Any]:
        """
        Anyify the value of map
        @return: the new anyfied map
        """
        return m
    @staticmethod
    def to_form(filter):
        """
        Parse filter into a form string

        @type filter: dict
        @param filter: object

        @return: the string
        """
        result = {}
        if filter:
            Utils._object_handler('', filter, result)
        return Form.to_form_string(result)

    @staticmethod
    def get_timestamp():
        """
        Get timestamp

        @return: the timestamp string
        """
        return datetime.utcnow().strftime("%Y-%m-%dT%H:%M:%SZ")

    @staticmethod
    def query(filter):
        """
        Parse filter into a object which's type is map[string]string

        @type filter: dict
        @param filter: query param

        @return: the object
        """
        out_dict = {}
        if filter:
            Utils._object_handler('', filter, out_dict)
        return out_dict

    @staticmethod
    def get_rpcsignature(signed_params, method, secret):
        """
        Get signature according to signedParams, method and secret

        @type signed_params: dict
        @param signed_params: params which need to be signed

        @type method: str
        @param method: http method e.g. GET

        @type secret: str
        @param secret: AccessKeySecret

        @return: the signature
        """
        queries = signed_params.copy()
        keys = list(queries.keys())
        keys.sort()

        canonicalized_query_string = ""

        for k in keys:
            if queries[k] is not None:
                canonicalized_query_string += f'&{quote(k, safe="~", encoding="utf-8")}=' \
                                              f'{quote(queries[k], safe="~", encoding="utf-8")}'

        string_to_sign = f'{method}&%2F&{quote_plus(canonicalized_query_string[1:], safe="~", encoding="utf-8")}'

        digest_maker = hmac.new(bytes(secret + '&', encoding="utf-8"),
                                bytes(string_to_sign, encoding="utf-8"),
                                digestmod=hashlib.sha1)
        hash_bytes = digest_maker.digest()
        signed_str = str(base64.b64encode(hash_bytes), encoding="utf-8")

        return signed_str

    @staticmethod
    def array_to_string_with_specified_style(array, prefix, style):
        """
        Parse array into a string with specified style

        @type array: any
        @param array: the array

        @type prefix: str
        @param prefix: the prefix string

        @param style: specified style e.g. repeatList

        @return: the string
        """
        if array is None:
            return ''

        if style == 'repeatList':
            return Utils._flat_repeat_list({prefix: array})
        elif style == 'simple':
            return ','.join(map(str, array))
        elif style == 'spaceDelimited':
            return ' '.join(map(str, array))
        elif style == 'pipeDelimited':
            return '|'.join(map(str, array))
        elif style == 'json':
            return DaraCore.to_json_string(Utils._parse_to_dict(array))
        else:
            return ''

    @staticmethod
    def _flat_repeat_list(dic):
        query = {}
        if dic:
            Utils._object_handler('', dic, query)

        l = []
        q = sorted(query)
        for i in q:
            k = quote_plus(i, encoding='utf-8')
            v = quote_plus(query[i], encoding='utf-8')
            l.append(k + '=' + v)
        return '&&'.join(l)

    @staticmethod
    def parse_to_map(inp):
        """
        Transform input as map.
        """
        try:
            result = Utils._parse_to_dict(inp)
            return copy.deepcopy(result)
        except TypeError:
            return

    @staticmethod
    def _parse_to_dict(val):
        if isinstance(val, dict):
            result = {}
            for k, v in val.items():
                if isinstance(v, (list, dict, DaraModel)):
                    result[k] = Utils._parse_to_dict(v)
                else:
                    result[k] = v
            return result
        elif isinstance(val, list):
            result = []
            for i in val:
                if isinstance(i, (list, dict, DaraModel)):
                    result.append(Utils._parse_to_dict(i))
                else:
                    result.append(i)
            return result
        elif isinstance(val, DaraModel):
            return val.to_map()

    @staticmethod
    def get_endpoint(endpoint, server_use, endpoint_type):
        """
        If endpointType is internal, use internal endpoint
        If serverUse is true and endpointType is accelerate, use accelerate endpoint
        Default return endpoint
        @param server_use whether use accelerate endpoint
        @param endpoint_type value must be internal or accelerate
        @return the final endpoint
        """
        if endpoint_type == "internal":
            str_split = endpoint.split('.')
            str_split[0] += "-internal"
            endpoint = ".".join(str_split)

        if server_use and endpoint_type == "accelerate":
            return "oss-accelerate.aliyuncs.com"

        return endpoint

    @staticmethod
    def hash(raw, sign_type):
        if sign_type == 'ACS3-HMAC-SHA256' or sign_type == 'ACS3-RSA-SHA256':
            return hashlib.sha256(raw).digest()
        elif sign_type == 'ACS3-HMAC-SM3':
            return hash_sm3(raw)

    @staticmethod
    def hex_encode(raw):
        if raw:
            return binascii.b2a_hex(raw).decode('utf-8')

    @staticmethod
    def get_authorization(request, sign_type, payload, ak, secret):
        canonical_uri = request.pathname if request.pathname else '/'
        canonicalized_query = get_canonical_query_string(request.query)
        canonicalized_headers, signed_headers = get_canonicalized_headers(request.headers)

        canonical_request = f'{request.method}\n' \
                            f'{canonical_uri}\n' \
                            f'{canonicalized_query}\n' \
                            f'{canonicalized_headers}\n' \
                            f'{signed_headers}\n' \
                            f'{payload}'

        str_to_sign = f'{sign_type}\n{Utils.hex_encode(Utils.hash(canonical_request.encode("utf-8"), sign_type))}'
        signature = Utils.hex_encode(signature_method(secret, str_to_sign, sign_type))
        auth = f'{sign_type} Credential={ak},SignedHeaders={signed_headers},Signature={signature}'
        return auth

    @staticmethod
    def get_encode_path(path):
        return quote(path, safe='/~', encoding="utf-8")

    @staticmethod
    def get_encode_param(param):
        return quote(param, safe='~', encoding="utf-8")


    @staticmethod
    def get_nonce() -> str:
        """
        Generate a nonce string
        @return: the nonce string
        """
        global _seqId
        thread_id = threading.get_ident()
        current_time = int(time.time() * 1000)
        seq = _seqId
        _seqId += 1
        randNum = random.getrandbits(64)
        _process_start_time = int(time.time() * 1000)
        msg = f'{_process_start_time}-{thread_id}-{current_time}-{seq}-{randNum}'
        md5 = hashlib.md5()
        md5.update(msg.encode('utf-8'))
        return md5.hexdigest()

    @staticmethod
    def get_date_utcstring() -> str:
        """
        Get an UTC format string by current date, e.g. 'Thu, 06 Feb 2020 07:32:54 GMT'
        @return: the UTC format string
        """
        return datetime.utcnow().strftime('%a, %d %b %Y %H:%M:%S GMT')

    @staticmethod
    def stringify_map_value(
        m: Dict[str, Any],
    ) -> Dict[str, str]:
        """
        Stringify the value of map
        @return: the new stringified map
        """
        if m is None:
            return {}

        dic_result = {}
        for k, v in m.items():
            if v is not None:
                if isinstance(v, bytes):
                    v = v.decode('utf-8')
                else:
                    v = str(v)
            dic_result[k] = v
        return dic_result

    @staticmethod
    def to_array(
        input: Any,
    ) -> List[Dict[str, Any]]:
        """
        Transform input as array.
        """
        if input is None:
            return []

        out = []
        for i in input:
            if isinstance(i, DaraModel):
                out.append(i.to_map())
            else:
                out.append(i)
        return out

    @staticmethod
    def __get_default_agent():
        return f'AlibabaCloud ({platform.system()}; {platform.machine()}) ' \
               f'Python/{platform.python_version()} Core/{Tea.__version__} TeaDSL/2'
               
    @staticmethod
    def get_user_agent(
        user_agent: str,
    ) -> str:
        """
        Get user agent, if it userAgent is not null, splice it with defaultUserAgent and return, otherwise return defaultUserAgent
        @return: the string value
        """
        if user_agent:
            return f'{Utils.__get_default_agent()} {user_agent}'
        return Utils.__get_default_agent()

    @staticmethod
    def get_endpoint_rules(product, region_id, endpoint_type, network, suffix=None):
        product = product or ""
        network = network or ""
        if endpoint_type == "regional":
            if region_id is None or region_id == "":
                raise RuntimeError(
                    "RegionId is empty, please set a valid RegionId")
            result = "<product><network>.<region_id>.aliyuncs.com".replace(
                "<region_id>", region_id)
        else:
            result = "<product><network>.aliyuncs.com"

        result = result.replace("<product>", product.lower())
        if network == "" or network == "public":
            result = result.replace("<network>", "")
        else:
            result = result.replace("<network>", "-"+network)
        return result
    
    
    @staticmethod
    def get_throttling_time_left(headers: Dict[str, str]) -> int:
        """
        Get throttling time left based on the response headers
        
        @param headers: The response headers
        @return: Remaining time in milliseconds before the throttle is lifted
        """
        rate_limit_user_api = headers.get("x-ratelimit-user-api")
        rate_limit_user = headers.get("x-ratelimit-user")

        time_left_user_api = Utils._get_time_left(rate_limit_user_api)
        time_left_user = Utils._get_time_left(rate_limit_user)

        return max(time_left_user_api, time_left_user)

    @staticmethod
    def _get_time_left(rate_limit: str) -> int:
        """
        Extract time left from rate limit string
        
        @param rate_limit: Rate limit string from headers
        @return: Time left in milliseconds
        """
        if rate_limit:
            pairs = rate_limit.split(',')
            for pair in pairs:
                key, value = pair.split(':')
                if key.strip() == 'TimeLeft':
                    return int(value.strip())
        return 0

    @staticmethod
    def flat_map(params: Dict[str, Any], prefix: str = '') -> Dict[str, str]:
        """
        Flatten the dictionary with a given prefix
        
        @param params: Dictionary to flatten
        @param prefix: Prefix for keys in the flattened dictionary
        @return: A flattened dictionary
        """
        flat_result = {}

        def _flatten(current_params, current_prefix):
            if isinstance(current_params, dict):
                for k, v in current_params.items():
                    new_key = f"{current_prefix}.{k}" if current_prefix else k
                    _flatten(v, new_key)
            elif isinstance(current_params, list):
                for index, item in enumerate(current_params):
                    new_key = f"{current_prefix}.{index + 1}"
                    _flatten(item, new_key)
            else:
                flat_result[current_prefix] = str(current_params)

        _flatten(params, prefix)
        return flat_result

    @staticmethod
    def map_to_flat_style(input: Any) -> Any:
        """
        Convert input to a flat style
        
        @param input: Input to convert
        @return: A flat representation of the input
        """
        if isinstance(input, dict):
            return Utils.flat_map(input)
        elif isinstance(input, list):
            flat_result = {}
            for index, item in enumerate(input):
                flat_result[index + 1] = str(item)
            return flat_result
        else:
            return str(input)