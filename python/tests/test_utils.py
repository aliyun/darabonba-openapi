import unittest
import os
import binascii
from io import BytesIO

from alibabacloud_tea_openapi.utils import signature_method, get_canonical_query_string
from alibabacloud_tea_openapi.utils import Utils as Client
from darabonba.request import DaraRequest
from darabonba.model import DaraModel
from tests.models import SourceModel, SourceModelUrlListObject, TargetModel

module_path = os.path.dirname(__file__)


class TestUtils(unittest.TestCase):
    class TestModel(DaraModel):
        def __init__(self):
            self.test_a = 'a'
            self.test_b = 'b'

        def validate(self):
            raise ValueError('test validate')

        def to_map(self):
            return {
                'test_a': self.test_a,
                'test_b': self.test_b
            }

    class TestConvertModel(DaraModel):
        def __init__(self):
            self.requestId = "test"
            self.dic = {}
            self.no_map = 1
            self.sub_model = None
            self.file = None

        def to_map(self):
            dic = {
                'requestId': self.requestId,
                'dic': self.dic,
                'no_map': self.no_map,
                'sub_model': self.sub_model,
                'file': self.file
            }
            return dic

    class TestConvertSubModel(DaraModel):
        def __init__(self):
            self.requestId = "subTest"
            self.id = 2

        def to_map(self):
            dic = {
                'requestId': self.requestId,
                'id': self.id
            }
            return dic

    class TestConvertMapModel(DaraModel):
        def __init__(self):
            self.requestId = ""
            self.extendId = 0
            self.dic = {}
            self.sub_model = None

        def to_map(self):
            dic = {
                'requestId': self.requestId,
                'dic': self.dic,
                'extendId': self.extendId,
                'sub_model': self.sub_model,
            }
            return dic

        def from_map(self, dic):
            self.requestId = dic.get("requestId") or ""
            self.extendId = dic.get("extendId") or 0
            self.dic = dic.get("dic")
            self.sub_model = dic.get("sub_model")

    def test_get_rpc_signature(self):
        query = {
            'query': 'test',
            'body': 'test'
        }
        result = Client.get_rpcsignature(query, 'GET', 'secret')
        self.assertEqual("XlUyV4sXjOuX5FnjUz9IF9tm5rU=", result)

    def test_get_timestamp(self):
        self.assertIsNotNone(Client.get_timestamp())

        self.assertIn("T", Client.get_timestamp())

        self.assertIn("Z", Client.get_timestamp())

    def test_query(self):
        result = Client.query(None)
        self.assertEqual(0, len(result))
        dic = {
            'str_test': 'test',
            'none_test': None,
            'int_test': 1
        }
        result = Client.query(dic)
        self.assertEqual('test', result.get('str_test'))
        self.assertIsNone(result.get("none_test"))
        self.assertEqual("1", result.get("int_test"))
        with open(os.path.join(module_path, "test.txt")) as f:
            fl = [1, None]
            sub_dict_fl = {
                'none_test': None,
                'int_test': 2,
                'str_test': 'test',
                'file_test': f
            }
            fl.append(sub_dict_fl)
            sl = [1, None]
            fl.append(sl)
            dic['list'] = fl
            result = Client.query(dic)
        self.assertEqual("1", result.get("list.1"))
        self.assertIsNone(result.get("list.2"))
        self.assertEqual("1", result.get("int_test"))
        self.assertEqual("2", result.get("list.3.int_test"))
        self.assertEqual(None, result.get("list.3.file_test"))
        self.assertIsNone(result.get("list.3.none_test"))
        self.assertEqual("test", result.get("list.3.str_test"))
        self.assertEqual("1", result.get("list.4.1"))

        sub_map_fd = {
            'none_test': None,
            'int_test': 2,
            'str_test': 'test'
        }
        fd = {
            'first_map_map': sub_map_fd,
            'first_map_list': sl,
            'none_test': None,
            'int_test': 2,
            'str_test': 'test',
            'model_test': self.TestConvertModel()
        }
        dic['map'] = fd

        result = Client.query(dic)
        self.assertEqual("1", result.get("map.first_map_list.1"))
        self.assertIsNone(result.get("map.none_test"))
        self.assertEqual("2", result.get("map.int_test"))
        self.assertEqual("test", result.get("map.str_test"))
        self.assertEqual('1', result.get("map.model_test.no_map"))
        self.assertIsNone(result.get("map.first_map_map.none_test"))
        self.assertEqual("2", result.get("map.first_map_map.int_test"))
        self.assertEqual("test", result.get("map.first_map_map.str_test"))

    def test_get_string_to_sign(self):
        request = DaraRequest()
        str_to_sign = Client.get_string_to_sign(request)
        self.assertEqual('GET\n\n\n\n\n', str_to_sign)

        request = DaraRequest()
        request.method = "POST"
        request.query = {
            'test': 'tests'
        }
        str_to_sign = Client.get_string_to_sign(request)
        self.assertEqual('POST\n\n\n\n\n?test=tests', str_to_sign)

        request = DaraRequest()
        request.headers = {
            'content-md5': 'md5',
        }
        str_to_sign = Client.get_string_to_sign(request)
        self.assertEqual('GET\n\nmd5\n\n\n', str_to_sign)

        request = DaraRequest()
        request.pathname = "Pathname"
        request.query = {
            'ccp': 'ok',
            'test': 'tests',
            'test1': ''
        }
        request.headers = {
            'x-acs-meta': 'user',
            "accept": "application/json",
            'content-md5': 'md5',
            'content-type': 'application/json',
            'date': 'date'
        }
        str_to_sign = Client.get_string_to_sign(request)
        s = 'GET\napplication/json\nmd5\napplication/json\ndate\nx-acs-meta:user\nPathname?ccp=ok&test=tests&test1'
        self.assertEqual(s, str_to_sign)

    def test_get_roa_signature(self):
        request = DaraRequest()
        str_to_sign = Client.get_string_to_sign(request)
        signature = Client.get_roasignature(str_to_sign, 'secret')
        self.assertEqual('GET\n\n\n\n\n', str_to_sign)
        self.assertEqual('XGXDWA78AEvx/wmfxKoVCq/afWw=', signature)

    def test_to_form(self):
        filter = {
            'client': 'test',
            'client1': None,
            'strs': ['str1', 'str2'],
            'tag': {
                'key': 'value'
            }
        }
        result = Client.to_form(filter)
        self.assertEqual('client=test&strs.1=str1&strs.2=str2&tag.key=value', result)

    def test_convert(self):
        filename = module_path + "/test.txt"
        with open(filename) as f:
            model = TestUtils.TestConvertModel()
            model.dic["key"] = "value"
            model.dic["testKey"] = "testValue"
            sub_model = TestUtils.TestConvertSubModel()
            model.sub_model = sub_model
            model.file = f
            map_model = TestUtils.TestConvertMapModel()
            Client.convert(model, map_model)
            self.assertIsNotNone(map_model)
            self.assertEqual("test", map_model.requestId)
            self.assertEqual(0, map_model.extendId)
        by = bytes('test', 'utf-8')
        stream = BytesIO()
        stream.write(by)
        url_list_object = SourceModelUrlListObject(
            url_object=stream
        )
        source = SourceModel(
            test='test',
            body_object=stream,
            list_object=[
                stream
            ],
            url_list_object=[
                url_list_object
            ]
        )
        target = TargetModel()
        Client.convert(source, target)
        self.assertEqual("test", target.test)
        self.assertIsNone(target.empty)
        self.assertIsNone(target.body)
        self.assertIsNotNone(len(target.list))
        self.assertIsNotNone(target.url_list[0])
        self.assertIsNone(target.url_list[0].url)

    def test_array_to_string_with_specified_style(self):
        array = ['ok', 'test', 2, 3]
        prefix = 'instance'
        t1 = Client.array_to_string_with_specified_style(array, prefix, 'repeatList')
        t2 = Client.array_to_string_with_specified_style(array, prefix, 'json')
        t3 = Client.array_to_string_with_specified_style(array, prefix, 'simple')
        t4 = Client.array_to_string_with_specified_style(array, prefix, 'spaceDelimited')
        t5 = Client.array_to_string_with_specified_style(array, prefix, 'pipeDelimited')
        t6 = Client.array_to_string_with_specified_style(array, prefix, 'piDelimited')
        t7 = Client.array_to_string_with_specified_style(None, prefix, 'pipeDelimited')
        self.assertEqual('instance.1=ok&&instance.2=test&&instance.3=2&&instance.4=3', t1)
        self.assertEqual('["ok","test",2,3]', t2)
        self.assertEqual('ok,test,2,3', t3)
        self.assertEqual('ok test 2 3', t4)
        self.assertEqual('ok|test|2|3', t5)
        self.assertEqual('', t6)
        self.assertEqual('', t7)

        model = self.TestConvertSubModel()
        res = Client.array_to_string_with_specified_style({'requestId': 'subTest', 'id': 2}, prefix, 'json')
        self.assertEqual('{"requestId":"subTest","id":2}', res)
        res = Client.array_to_string_with_specified_style(model, prefix, 'json')
        self.assertEqual('{"requestId":"subTest","id":2}', res)
        res = Client.array_to_string_with_specified_style([model], prefix, 'json')
        self.assertEqual('[{"requestId":"subTest","id":2}]', res)
        res = Client.array_to_string_with_specified_style({'model': model}, prefix, 'json')
        self.assertEqual('{"model":{"requestId":"subTest","id":2}}', res)

    def test_parse_to_map(self):
        self.assertIsNone(Client.parse_to_map(None))

        filename = module_path + "/test.txt"
        with open(filename) as f:
            res = Client.parse_to_map({'file': f})
            self.assertIsNone(res)

        res = Client.parse_to_map({"key": "value"})
        self.assertEqual('value', res['key'])

        model = self.TestConvertSubModel()
        res = Client.parse_to_map(model)
        self.assertEqual('subTest', res['requestId'])
        self.assertEqual(2, res['id'])

        res = Client.parse_to_map({
            "key": "value",
            'model': model
        })
        self.assertEqual('value', res['key'])
        self.assertEqual('subTest', res['model']['requestId'])
        self.assertEqual(2, res['model']['id'])

        res = Client.parse_to_map({
            'model_list': [model, model, 'model'],
            'model_dict': {"model1": model, "model2": model}
        })
        self.assertEqual('subTest', res['model_list'][0]['requestId'])
        self.assertEqual(2, res['model_list'][1]['id'])
        self.assertEqual('model', res['model_list'][2])
        self.assertEqual('subTest', res['model_dict']['model1']['requestId'])
        self.assertEqual(2, res['model_dict']['model2']['id'])

    def test_get_endpoint(self):
        self.assertEqual("test", Client.get_endpoint("test", False, ""))

        self.assertEqual("test-internal.endpoint", Client.get_endpoint("test.endpoint", False, "internal"))

        self.assertEqual("oss-accelerate.aliyuncs.com", Client.get_endpoint("test", True, "accelerate"))

    def test_hex_encode(self):
        # ACS3 - HMAC - SHA256
        res = Client.hex_encode(
            Client.hash(b'test', 'ACS3-HMAC-SHA256')
        )
        self.assertEqual(
            '9f86d081884c7d659a2feaa0c55ad015a3bf4f1b2b0b822cd15d6c15b0f00a08',
            res
        )
        # ACS3 - RSA - SHA256
        res = Client.hex_encode(
            Client.hash(b'test', 'ACS3-RSA-SHA256')
        )
        self.assertEqual(
            '9f86d081884c7d659a2feaa0c55ad015a3bf4f1b2b0b822cd15d6c15b0f00a08',
            res
        )
        # ACS3 - HMAC - SM3
        res = Client.hex_encode(
            Client.hash(b'test', 'ACS3-HMAC-SM3')
        )
        self.assertEqual(
            '55e12e91650d2fec56ec74e1d3e4ddbfce2ef3a65890c2a19ecf88a307e76a23',
            res
        )

        res = Client.hex_encode(
            Client.hash(b'test', 'ACS3-SHA256')
        )
        self.assertEqual(
            None,
            res
        )

    def test_get_authorization(self):
        # request method is 'GET'
        request = DaraRequest()
        request.query = {
            'test': 'ok',
            'empty': ''
        }
        request.headers = {
            'x-acs-test': 'http',
            'x-acs-TEST': 'https'
        }

        res = Client.get_authorization(
            request,
            'ACS3-HMAC-SHA256',
            '55e12e91650d2fec56ec74e1d3e4ddbfce2ef3a65890c2a19ecf88a307e76a23',
            'acesskey',
            'secret'
        )
        self.assertEqual(
            'ACS3-HMAC-SHA256 Credential=acesskey,SignedHea'
            'ders=x-acs-test,Signature=d16b30a7699ae9e43875b13195b2f81bcc3ed10c14a9b5eb780e51619aa50be1',
            res
        )

    def test_get_encode_path(self):
        res = Client.get_encode_path('/path/ test')
        self.assertEqual('/path/%20test', res)

    def test_get_encode_param(self):
        res = Client.get_encode_param('a/b/c/ test')
        self.assertEqual('a%2Fb%2Fc%2F%20test', res)

    def test_signature_method(self):
        pri_key = '-----BEGIN RSA PRIVATE KEY-----\nMIICdgIBADANBgkqhkiG9w0BAQEFAASCAmAwggJcAgEAAo' \
                  'GBAKzSQmrnH0YnezZ98NK50WjMuci0hgGVcSthIZOTWMIy' \
                  'SznY9Jj1hlvek7W0uYagtFHz03BHQnHAb5Xs0DZm0Sj9+5' \
                  'r79GggwEzTJDYEsLyFwXM3ZOIxqxL4sRg94MHsa81M9NXG' \
                  'HMyMvvffQTn1OBVLTVz5jgJ48foMn7j7r9kRAgMBAAECgY' \
                  'EAnZppw3/ef2XF8Z3Mnv+iP0ZkLuqiQpN8TykXK7P1/7NJ' \
                  '8wktlshhrSo/3jdf8axghVQsgHob2Ay8Nidugg4lsxILAU' \
                  'BHvfQsQp1MAWvxslsVj+ddw01MQnt8kHmC/qhok+YuNqqA' \
                  'GBcoD6cthRUjEri6hfs599EfPs2DcWW06qECQQDfNqUUhc' \
                  'DQ/SQHRhfY9UIlaSEs2CVagDrSYFG1wyG+PXDSMes9ZRHs' \
                  'vVVBmNGmtUTg/jioTU3yuPsis5s9ppbVAkEAxjTAQxv5lBB' \
                  'm/ikMTzPShljxDZnXh6lKWG9gR1p5fKoQTzLyyhHzkBSFe' \
                  '848sMm68HWCX2wgIpQLHj0GccYPTQJAduMKBeY/jpBlkiI' \
                  '5LWtj8b0O2G2/Z3aI3ehDXQYzgLoEz0+bNbYRWAB32lpkv' \
                  '+AocZW1455Y+ACichcrhiimiQJAW/6L5hoL4u8h/oFq1zAE' \
                  'XJrXdyqaYLrwaM947mVN0dDVNQ0+pw9h7tO3iNkWTi+zdnv' \
                  '0APociDASYPyOCyyUWQJACMNRM1/rboXuKfMmVjmmz0XhaD' \
                  'UC/JkqSwIiaZi+47M21e9BTp1218NA6VaPgJJHeJr4sNOnY' \
                  'sx+1cwXO5cuZg==\n-----END RSA PRIVATE KEY-----'
        res = signature_method("secret", "source", "ACS3-HMAC-SM3")
        self.assertEqual(b'b9ff646822f41ef647c1416fa2b8408923828abc0464af6706e18db3e8553da8', binascii.b2a_hex(res))

        res = signature_method(pri_key, "source", "ACS3-RSA-SHA256")
        self.assertEqual(b'a00b88ae04f651a8ab645e724949ff435bbb2cf9a'
                         b'37aa54323024477f8031f4e13dc948484c5c5a81ba'
                         b'53a55eb0571dffccc1e953c93269d6da23ed319e0f'
                         b'1ef699bcc9823a646574628ae1b70ed569b5a07d13'
                         b'9dda28996b5b9231f5ba96141f0893deec2fbf54a0'
                         b'fa2c203b8ae74dd26f457ac29c873745a5b88273d2b3d12', binascii.b2a_hex(res))

    def test_get_canonical_query_string(self):
        self.assertEqual('test=%20~%2F%2A-%2B', get_canonical_query_string({'test': ' ~/*-+'}))

    def test_stringify_map_value(self):
        self.assertEqual({}, Client.stringify_map_value(None))
        self.assertEqual({}, Client.stringify_map_value({}))
        dic = {
            'test': 100,
            'bkey': b'bytes',
            'key': None
        }
        self.assertEqual("100", Client.stringify_map_value(dic)["test"])

    def test_get_nonce(self):
        self.assertIsNotNone(Client.get_nonce())

    def test_to_array(self):
        tm = self.TestModel()
        lis = [tm, tm]
        res = Client.to_array(lis)
        self.assertEqual('a', res[0]['test_a'])
        res = Client.to_array(None)
        self.assertEqual([], res)
        lis = ['tm', 'tm']
        res = Client.to_array(lis)
        self.assertEqual(lis, res)

    def test_get_date_utc_string(self):
        self.assertIn('GMT', Client.get_date_utcstring())

    def test_get_user_agent(self):
        self.assertIsNotNone(Client.get_user_agent(''))
        self.assertIn("test", Client.get_user_agent("test"))

    def test_get_endpoint_rules(self):
        with self.assertRaises(RuntimeError):
            Client.get_endpoint_rules("ecs", "", "regional", "")

        self.assertEqual("ecs.cn-hangzhou.aliyuncs.com",
                         Client.get_endpoint_rules("ecs", "cn-hangzhou", "regional", ""))

        self.assertEqual("ecs-intl.cn-hangzhou.aliyuncs.com",
                         Client.get_endpoint_rules("ecs", "cn-hangzhou", "regional", "intl"))

        self.assertEqual("ecs.aliyuncs.com", Client.get_endpoint_rules(
            "ecs", "cn-hangzhou", "central", ""))

        self.assertEqual("ecs.aliyuncs.com", Client.get_endpoint_rules(
            "ecs", "cn-hangzhou", "central", "public"))

        self.assertEqual("ecs-intl.aliyuncs.com",
                         Client.get_endpoint_rules("ecs", "cn-hangzhou", "central", "intl"))

    def test_map_to_flat_style(self):
        # Test null
        self.assertIsNone(Client.map_to_flat_style(None))

        # Test primitive values
        self.assertEqual("test", Client.map_to_flat_style("test"))
        self.assertEqual(123, Client.map_to_flat_style(123))
        self.assertEqual(True, Client.map_to_flat_style(True))

        # Test plain dict
        plain_map = {
            "key1": "value1",
            "key2": "value2"
        }
        flat_map = Client.map_to_flat_style(plain_map)
        self.assertEqual("value1", flat_map["#4#key1"])
        self.assertEqual("value2", flat_map["#4#key2"])

        # Test nested dict
        nested_map = {
            "outerKey": {
                "innerKey": "innerValue"
            }
        }
        flat_nested_map = Client.map_to_flat_style(nested_map)
        self.assertEqual("innerValue", flat_nested_map["#8#outerKey"]["#8#innerKey"])

        # Test list
        arr = ["item1", "item2"]
        flat_arr = Client.map_to_flat_style(arr)
        self.assertEqual("item1", flat_arr[0])
        self.assertEqual("item2", flat_arr[1])

        # Test list with dict elements
        arr_with_dict = [
            {"key": "value"}
        ]
        flat_arr_with_dict = Client.map_to_flat_style(arr_with_dict)
        self.assertEqual("value", flat_arr_with_dict[0]["#3#key"])

        # Test DaraModel
        model_with_tags = self.TestConvertMapModel()
        model_with_tags.requestId = "testName"
        model_with_tags.dic = {"tagKey": "tagValue"}
        
        flat_model = Client.map_to_flat_style(model_with_tags)
        # Should return the same object (modified in place)
        self.assertIs(flat_model, model_with_tags)
        self.assertEqual("testName", flat_model.requestId)
        self.assertEqual("tagValue", flat_model.dic["#6#tagKey"])

        # Test list of DaraModels
        model2 = self.TestConvertMapModel()
        model2.requestId = "test2"
        model2.dic = {"key2": "value2"}
        model_list = [model2]
        flat_model_list = Client.map_to_flat_style(model_list)
        self.assertEqual("test2", flat_model_list[0].requestId)
        self.assertEqual("value2", flat_model_list[0].dic["#4#key2"])