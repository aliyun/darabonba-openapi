import * as assert from 'assert';
import * as http from 'http';
import { AddressInfo } from 'net';
import { Readable } from 'stream';
import * as $dara from '@darabonba/typescript';
import Client from '../src/client';
import { Config } from '../src/utils';

// 与跨包产品 Client 一样，仅通过子类访问 protected 上传辅助方法。
class ProductClient extends Client {
  resolve(runtime: $dara.RuntimeOptions) { return this.getOSSPrivateUrl(runtime); }
  upload(url: string, runtime = new $dara.RuntimeOptions()) {
    return this.postOSSObjectWithURL(url, 'test-bucket', {
      host: 'oss-cn-hangzhou.aliyuncs.com', OSSAccessKeyId: 'test-id',
      policy: 'test-policy', Signature: 'test-signature', key: 'dir/file.bin',
      success_action_status: '201', file: new $dara.FileField({
        filename: 'dir/file.bin', contentType: 'application/octet-stream',
        content: Readable.from([Buffer.from([0, 1, 255, 10])]),
      }),
    }, runtime);
  }
}

describe('OSS PrivateLink upload', function () {
  let server: http.Server;
  let endpoint: string;
  let status: number;
  let records: Array<{ method: string, target: string, host: string, body?: Buffer }>;
  const client = new ProductClient(new Config({}));

  beforeEach(done => {
    status = 201;
    records = [];
    server = http.createServer((request, response) => {
      const chunks: Buffer[] = [];
      request.on('data', chunk => chunks.push(Buffer.from(chunk)));
      request.on('end', () => {
        records.push({ method: request.method, target: request.url, host: request.headers.host, body: Buffer.concat(chunks) });
        response.writeHead(status, { 'Content-Type': 'application/xml' });
        response.end(status === 201 ? '<PostResponse><Key>dir/file.bin</Key></PostResponse>' :
          '<Error><Code>AccessDenied</Code><Message>denied</Message><RequestId>req-id</RequestId><HostId>host-id</HostId></Error>');
      });
    });
    server.listen(0, '127.0.0.1', () => {
      endpoint = 'http://127.0.0.1:' + (server.address() as AddressInfo).port;
      done();
    });
  });
  afterEach(done => server.close(done));

  it('Runtime 优先、空值回退与 Config 映射', () => {
    const configured = new ProductClient(new Config({ ossPrivateUrl: 'http://config.example' }));
    assert.strictEqual(configured.resolve(new $dara.RuntimeOptions()), 'http://config.example');
    assert.strictEqual(configured.resolve(new $dara.RuntimeOptions({ ossPrivateUrl: '' })), 'http://config.example');
    assert.strictEqual(configured.resolve(new $dara.RuntimeOptions({ ossPrivateUrl: 'https://runtime.example' })), 'https://runtime.example');
    assert.strictEqual(client.resolve(new $dara.RuntimeOptions()), undefined);
    assert.strictEqual($dara.toMap(new Config({ ossPrivateUrl: endpoint })).ossPrivateUrl, endpoint);
  });

  it('Path-style、前缀及 multipart 二进制保持', async () => {
    for (const prefix of ['', '/', '/proxy', '/proxy/']) {
      const result = await client.upload(endpoint + prefix);
      assert.strictEqual(result.PostResponse.Key, 'dir/file.bin');
      const record = records[records.length - 1];
      assert.strictEqual(record.method, 'POST');
      assert.strictEqual(record.host, endpoint.slice(7));
      assert.strictEqual(record.target, prefix.replace(/\/$/, '') + '/test-bucket/');
      for (const name of ['OSSAccessKeyId', 'policy', 'Signature', 'key', 'success_action_status', 'file']) {
        assert.ok(record.body.includes(Buffer.from('name="' + name + '"')));
      }
      assert.ok(record.body.includes(Buffer.from('test-policy')));
      assert.ok(record.body.includes(Buffer.from([0, 1, 255, 10])));
    }
  });

  it('XML 错误保留 code、message、statusCode 及 OSS 标识', async () => {
    status = 403;
    await assert.rejects(client.upload(endpoint), (error: any) => {
      assert.strictEqual(error.code, 'AccessDenied');
      assert.strictEqual(error.message, 'AccessDenied: denied');
      assert.strictEqual(error.statusCode, 403);
      assert.strictEqual(error.data.httpCode, 403);
      assert.strictEqual(error.data.requestId, 'req-id');
      assert.strictEqual(error.data.hostId, 'host-id');
      return true;
    });
    assert.strictEqual(records.length, 1);
  });

  it('无效地址统一报错且不发送网络请求', async () => {
    for (const url of [null, '', 'example.com', '//example.com', 'ftp://example.com', 'https:///prefix', 'http://',
      'http://example.com:99999', 'http://user@example.com', 'http://user:password@example.com', 'http://@example.com',
      'http://example.com/?a=b', 'http://example.com?', 'http://example.com/#fragment', 'http://example.com#']) {
      await assert.rejects(client.upload(url), (error: any) => error.code === 'InvalidOSSPrivateUrl');
    }
    assert.strictEqual(records.length, 0);
  });

  it('HTTPS 传输 URL 保留协议和显式端口', async () => {
    // 在 HTTP 传输边界记录真实运行时构造的 URL，避免访问公网。
    const transport = require('module').createRequire(require.resolve('@darabonba/typescript'))('httpx');
    const original = transport.request;
    const targets: string[] = [];
    transport.request = async (url: string) => {
      targets.push(url);
      throw new Error('测试在传输边界终止');
    };
    try {
      for (const [url, target] of [['https://pvl.example/', 'https://pvl.example/test-bucket/'],
        ['https://pvl.example:8443/proxy', 'https://pvl.example:8443/proxy/test-bucket/']]) {
        await assert.rejects(client.upload(url));
        assert.strictEqual(targets[targets.length - 1], target);
      }
    } finally {
      transport.request = original;
    }
    assert.strictEqual(records.length, 0);
  });
});
