import unittest

from alibabacloud_gateway_spi.models import AttributeMap
from alibabacloud_tea_openapi.client import Client
from alibabacloud_tea_openapi.models import Config


class TestEndpointSuffixPriority(unittest.TestCase):
    def test_blank_attribute_uses_config_without_mutating_caller(self):
        client = Client(Config(endpoint='example.com', endpoint_suffix='aliyuncs.alibaba'))
        for value in (None, '', ' \t\r\n', 'aliyuncs.com'):
            original = AttributeMap(key={'endpointSuffix': value}, attributes={'kept': 'value'})
            client._attribute_map = original
            mapped = client._attribute_map_with_endpoint_suffix()
            self.assertEqual(value if value and value.strip() else 'aliyuncs.alibaba', mapped.key['endpointSuffix'])
            self.assertEqual(value, original.key['endpointSuffix'])
            if not value or not value.strip():
                self.assertIsNot(original, mapped)
                mapped.attributes['kept'] = 'changed'
                self.assertEqual('value', original.attributes['kept'])
            else:
                self.assertIs(original, mapped)
