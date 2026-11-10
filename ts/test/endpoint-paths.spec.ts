import assert from 'assert';
import 'mocha';
import OpenApi from '../src/client';
import * as Models from '../src/utils';
import * as dara from '@darabonba/typescript';

describe('endpoint suffix request paths', () => {
  it('keeps suffix priority, normalizes values and rejects invalid values', () => {
    const previous = process.env.ALIBABA_CLOUD_ENDPOINT_SUFFIX;
    try {
      delete process.env.ALIBABA_CLOUD_ENDPOINT_SUFFIX;
      const client = new OpenApi(new Models.Config({ endpoint: 'ecs.aliyuncs.com' }));
      const resolve = () => (client as any).endpointForRequest(new Models.OpenApiRequest({}));
      assert.strictEqual(resolve(), 'ecs.aliyuncs.com');
      process.env.ALIBABA_CLOUD_ENDPOINT_SUFFIX = ' ALIYUNCS.ALIBABA ';
      assert.strictEqual(resolve(), 'ecs.aliyuncs.alibaba');
      client._endpointSuffix = 'aliyuncs.com';
      assert.strictEqual(resolve(), 'ecs.aliyuncs.com');
      (client as any)._attributeMap = { key: { endpointSuffix: ' ALIYUNCS.ALIBABA ' } };
      assert.strictEqual(resolve(), 'ecs.aliyuncs.alibaba');
      (client as any)._attributeMap.key.endpointSuffix = '  ';
      assert.strictEqual(resolve(), 'ecs.aliyuncs.com');
      (client as any)._attributeMap.key.endpointSuffix = 'evil.com';
      assert.throws(resolve, /endpointSuffix/);
      (client as any)._attributeMap = null;
      client._endpointSuffix = '';
      process.env.ALIBABA_CLOUD_ENDPOINT_SUFFIX = '  ';
      assert.strictEqual(resolve(), 'ecs.aliyuncs.com');
    } finally {
      if (previous === undefined) delete process.env.ALIBABA_CLOUD_ENDPOINT_SUFFIX;
      else process.env.ALIBABA_CLOUD_ENDPOINT_SUFFIX = previous;
    }
  });

  it('rewrites the actual outbound host for every direct request entry', async () => {
    const transport = require('@darabonba/typescript');
    const descriptor = Object.getOwnPropertyDescriptor(transport, 'doAction');
    let seen: string[] = [];
    Object.defineProperty(transport, 'doAction', { configurable: true, value: async (request: any) => {
      seen.push(request.headers.host);
      throw new Error('stop-before-network');
    } });
    try {
      const params = new Models.Params({ action: 'Test', version: '2020-01-01', protocol: 'HTTPS', method: 'GET', pathname: '/', authType: 'Anonymous', style: 'ROA', reqBodyType: 'json', bodyType: 'string' });
      const runtime = new dara.RuntimeOptions({ autoretry: false, maxAttempts: 1 });
      const paths = [
        (c: OpenApi, r: Models.OpenApiRequest) => c.doRPCRequest('Test', '2020-01-01', 'HTTPS', 'GET', 'Anonymous', 'string', r, runtime),
        (c: OpenApi, r: Models.OpenApiRequest) => c.doROARequest('Test', '2020-01-01', 'HTTPS', 'GET', 'Anonymous', '/', 'string', r, runtime),
        (c: OpenApi, r: Models.OpenApiRequest) => c.doROARequestWithForm('Test', '2020-01-01', 'HTTPS', 'GET', 'Anonymous', '/', 'string', r, runtime),
        (c: OpenApi, r: Models.OpenApiRequest) => c.doRequest(params, r, runtime),
        (c: OpenApi, r: Models.OpenApiRequest) => c.callApi(params, r, runtime),
        async (c: OpenApi, r: Models.OpenApiRequest) => { for await (const _ of c.callSSEApi(params, r, runtime)) { } },
      ];
      for (const path of paths) {
        for (const [endpoint, override, expected] of [
          ['ecs.cn-hangzhou.aliyuncs.com', '', 'ecs.cn-hangzhou.aliyuncs.alibaba'],
          ['ecs.aliyuncs.com', 'oss-cn-hangzhou.aliyuncs.com', 'oss-cn-hangzhou.aliyuncs.alibaba'],
          ['ecs.aliyun.com', '', 'ecs.aliyun.com'],
          ['ecs.aliyuncs.alibaba', '', 'ecs.aliyuncs.alibaba'],
        ]) {
          const client = new OpenApi(new Models.Config({ endpoint, endpointSuffix: 'aliyuncs.alibaba' }));
          seen = [];
          await assert.rejects(path(client, new Models.OpenApiRequest({ endpointOverride: override })));
          assert.deepStrictEqual(seen, [expected]);
          assert.strictEqual(client._endpoint, endpoint);
        }
      }
    } finally {
      Object.defineProperty(transport, 'doAction', descriptor);
    }
  });
});
