import asyncio
import unittest
from unittest import mock

from darabonba.core import DaraCore
from darabonba.runtime import RuntimeOptions
from alibabacloud_tea_openapi.client import Client
from alibabacloud_tea_openapi.models import Config, OpenApiRequest, Params


class EndpointPathsTests(unittest.TestCase):
    def test_suffix_priority_and_validation(self):
        from alibabacloud_gateway_spi.models import AttributeMap
        with mock.patch.dict('os.environ', {}, clear=True):
            import os
            client = Client(Config(endpoint='ecs.aliyuncs.com'))
            request = OpenApiRequest()
            self.assertEqual('ecs.aliyuncs.com', client._endpoint_for_request(request))
            os.environ['ALIBABA_CLOUD_ENDPOINT_SUFFIX'] = ' ALIYUNCS.ALIBABA '
            self.assertEqual('ecs.aliyuncs.alibaba', client._endpoint_for_request(request))
            client._endpoint_suffix = 'aliyuncs.com'
            self.assertEqual('ecs.aliyuncs.com', client._endpoint_for_request(request))
            client._attribute_map = AttributeMap(key={'endpointSuffix': ' ALIYUNCS.ALIBABA '})
            self.assertEqual('ecs.aliyuncs.alibaba', client._endpoint_for_request(request))
            client._attribute_map.key['endpointSuffix'] = '  '
            self.assertEqual('ecs.aliyuncs.com', client._endpoint_for_request(request))
            client._attribute_map.key['endpointSuffix'] = 'evil.com'
            with self.assertRaises(Exception):
                client._endpoint_for_request(request)

    def test_outbound_host_for_sync_and_async_entries(self):
        loop = asyncio.new_event_loop()
        self.addCleanup(loop.close)
        params = Params(action='Test', version='2020-01-01', protocol='HTTPS', method='GET', pathname='/', auth_type='Anonymous', style='ROA', req_body_type='json', body_type='string')
        runtime = RuntimeOptions(autoretry=False, max_attempts=1)
        seen = []

        def capture(request, *args, **kwargs):
            seen.append(request.headers['host'])
            raise RuntimeError('stop-before-network')

        async def capture_async(request, *args, **kwargs):
            return capture(request)

        async def consume_sse(client, request):
            async for _ in client.call_sseapi_async(params, request, runtime):
                pass

        class Gateway:
            def modify_configuration(self, context, attributes):
                pass

            def modify_request(self, context, attributes):
                context.request.headers['host'] = context.configuration.endpoint

            async def modify_configuration_async(self, context, attributes):
                self.modify_configuration(context, attributes)

            async def modify_request_async(self, context, attributes):
                self.modify_request(context, attributes)

        paths = [
            lambda c, r: c.do_rpcrequest('Test', '2020-01-01', 'HTTPS', 'GET', 'Anonymous', 'string', r, runtime),
            lambda c, r: c.do_roarequest('Test', '2020-01-01', 'HTTPS', 'GET', 'Anonymous', '/', 'string', r, runtime),
            lambda c, r: c.do_roarequest_with_form('Test', '2020-01-01', 'HTTPS', 'GET', 'Anonymous', '/', 'string', r, runtime),
            lambda c, r: c.do_request(params, r, runtime),
            lambda c, r: c.call_api(params, r, runtime),
            lambda c, r: list(c.call_sseapi(params, r, runtime)),
            lambda c, r: loop.run_until_complete(c.do_rpcrequest_async('Test', '2020-01-01', 'HTTPS', 'GET', 'Anonymous', 'string', r, runtime)),
            lambda c, r: loop.run_until_complete(c.do_roarequest_async('Test', '2020-01-01', 'HTTPS', 'GET', 'Anonymous', '/', 'string', r, runtime)),
            lambda c, r: loop.run_until_complete(c.do_roarequest_with_form_async('Test', '2020-01-01', 'HTTPS', 'GET', 'Anonymous', '/', 'string', r, runtime)),
            lambda c, r: loop.run_until_complete(c.do_request_async(params, r, runtime)),
            lambda c, r: loop.run_until_complete(c.call_api_async(params, r, runtime)),
            lambda c, r: loop.run_until_complete(consume_sse(c, r)),
            lambda c, r: c.execute(params, r, runtime),
            lambda c, r: loop.run_until_complete(c.execute_async(params, r, runtime)),
        ]
        with mock.patch.object(DaraCore, 'do_action', side_effect=capture), \
                mock.patch.object(DaraCore, 'async_do_action', side_effect=capture_async), \
                mock.patch.object(DaraCore, 'do_sse_action', side_effect=capture), \
                mock.patch.object(DaraCore, 'async_do_sse_action', side_effect=capture_async):
            for path in paths:
                for endpoint, override, expected in [
                    ('ecs.cn-hangzhou.aliyuncs.com', '', 'ecs.cn-hangzhou.aliyuncs.alibaba'),
                    ('ecs.aliyuncs.com', 'oss-cn-hangzhou.aliyuncs.com', 'oss-cn-hangzhou.aliyuncs.alibaba'),
                    ('ecs.aliyun.com', '', 'ecs.aliyun.com'),
                    ('ecs.aliyuncs.alibaba', '', 'ecs.aliyuncs.alibaba'),
                ]:
                    client = Client(Config(endpoint=endpoint, endpoint_suffix='aliyuncs.alibaba'))
                    client._spi = Gateway()
                    seen.clear()
                    with self.assertRaises(Exception) as caught:
                        path(client, OpenApiRequest(endpoint_override=override))
                    self.assertEqual([expected], seen, str(caught.exception))
                    self.assertEqual(endpoint, client._endpoint)
