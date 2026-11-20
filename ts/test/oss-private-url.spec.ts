import * as assert from 'assert';
import * as http from 'http';
import * as fs from 'fs';
import { AddressInfo } from 'net';
import { Readable } from 'stream';
import * as $dara from '@darabonba/typescript';
import Client from '../src/client';
import { Config } from '../src/utils';

// 与跨包产品 Client 一样，仅通过子类访问 protected 上传辅助方法。
class ProductClient extends Client {
  resolve(runtime: $dara.RuntimeOptions) { return this.getOSSPrivateUrl(runtime); }
  upload(url: string, runtime = new $dara.RuntimeOptions(), content = Readable.from([Buffer.from([0, 1, 255, 10])])) {
    return this.postOSSObjectWithURL(url, 'test-bucket', {
      host: 'oss-cn-hangzhou.aliyuncs.com', OSSAccessKeyId: 'test-id',
      policy: 'test-policy', Signature: 'test-signature', key: 'dir/file.bin',
      success_action_status: '201', file: new $dara.FileField({
        filename: 'dir/file.bin', contentType: 'application/octet-stream',
        content,
      }),
    }, runtime);
  }
}

describe('OSS PrivateLink upload', function () {
  let server: http.Server;
  let endpoint: string;
  let status: number;
  let failures: number;
  let records: Array<{ method: string, target: string, host: string, body?: Buffer, fileMode?: number, directoryMode?: number }>;
  const client = new ProductClient(new Config({}));
  const originalMkdtemp = fs.promises.mkdtemp;
  let tempDirectories: string[];
  const retryClient = () => new ProductClient(new Config({
    retryOptions: new $dara.RetryOptions({ retryable: true,
      retryCondition: [new $dara.RetryCondition({ maxAttempts: 2,
        exception: [], errorCode: ['AccessDenied'], maxDelay: 60000 })] }),
  }));

  beforeEach(done => {
    tempDirectories = [];
    fs.promises.mkdtemp = (async (prefix: string) => {
      const directory = await originalMkdtemp(prefix);
      tempDirectories.push(directory);
      return directory;
    }) as typeof fs.promises.mkdtemp;
    status = 201;
    failures = 0;
    records = [];
    server = http.createServer((request, response) => {
      const chunks: Buffer[] = [];
      request.on('data', chunk => chunks.push(Buffer.from(chunk)));
      request.on('end', () => {
        const directory = tempDirectories[tempDirectories.length - 1];
        records.push({ method: request.method, target: request.url, host: request.headers.host, body: Buffer.concat(chunks),
          fileMode: directory ? fs.statSync(directory + '/multipart').mode & 0o777 : undefined,
          directoryMode: directory ? fs.statSync(directory).mode & 0o777 : undefined });
        const responseStatus = failures > 0 ? (failures--, 403) : status;
        response.writeHead(responseStatus, { 'Content-Type': 'application/xml' });
        response.end(responseStatus === 201 ? '<PostResponse><Key>dir/file.bin</Key></PostResponse>' :
          '<Error><Code>AccessDenied</Code><Message>denied</Message><RequestId>req-id</RequestId><HostId>host-id</HostId></Error>');
      });
    });
    server.listen(0, '127.0.0.1', () => {
      endpoint = 'http://127.0.0.1:' + (server.address() as AddressInfo).port;
      done();
    });
  });
  afterEach(async () => {
    fs.promises.mkdtemp = originalMkdtemp;
    try {
      for (const directory of tempDirectories) {
        await assert.rejects(fs.promises.stat(directory), (error: any) => error.code === 'ENOENT');
      }
    } finally {
      await new Promise<void>(resolve => server.close(() => resolve()));
    }
  });

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

  it('上传失败按配置重试并保留 multipart 内容', async () => {
    failures = 1;
    const result = await retryClient().upload(endpoint);
    assert.strictEqual(result.PostResponse.Key, 'dir/file.bin');
    assert.strictEqual(records.length, 2);
    for (const record of records) {
      assert.strictEqual(record.target, '/test-bucket/');
      assert.ok(record.body.includes(Buffer.from('name="file"')));
      assert.ok(record.body.includes(Buffer.from([0, 1, 255, 10])));
    }
    assert.deepStrictEqual(records[0].body, records[1].body);
    assert.strictEqual(tempDirectories.length, 1);
    if (process.platform !== 'win32') {
      assert.strictEqual(records[0].fileMode, 0o600);
      assert.strictEqual(records[0].directoryMode, 0o700);
    }
  });

  it('大文件流重试逐字节一致且只建立一个临时副本', async () => {
    failures = 1;
    const chunk = Buffer.alloc(64 * 1024, 0xff);
    const content = Readable.from((async function* () {
      for (let i = 0; i < 128; i++) { yield chunk; }
    })());
    await retryClient().upload(endpoint, new $dara.RuntimeOptions(), content);
    assert.strictEqual(records.length, 2);
    assert.ok(records[0].body.length > 8 * 1024 * 1024);
    assert.deepStrictEqual(records[0].body, records[1].body);
    assert.strictEqual(tempDirectories.length, 1);
  });

  it('重试耗尽仍保留 OSS 错误并清理临时副本', async () => {
    status = 403;
    await assert.rejects(retryClient().upload(endpoint), (error: any) => {
      assert.strictEqual(error.code, 'AccessDenied');
      assert.strictEqual(error.statusCode, 403);
      return true;
    });
    assert.strictEqual(records.length, 2);
    assert.deepStrictEqual(records[0].body, records[1].body);
    assert.strictEqual(tempDirectories.length, 1);
  });

  it('文件源读取失败不上传、不重试且清理临时副本', async () => {
    const error = new Error('测试文件读取失败');
    const content = new Readable({ read() { this.destroy(error); } });
    await assert.rejects(retryClient().upload(endpoint, new $dara.RuntimeOptions(), content), error);
    assert.strictEqual(records.length, 0);
    assert.strictEqual(tempDirectories.length, 1);
  });

  it('临时文件创建失败仍清理目录且保留原始错误', async () => {
    const nativeFS = require('fs');
    const original = nativeFS.createWriteStream;
    const error = new Error('测试磁盘写入失败');
    nativeFS.createWriteStream = () => { throw error; };
    try {
      await assert.rejects(retryClient().upload(endpoint), error);
    } finally {
      nativeFS.createWriteStream = original;
    }
    assert.strictEqual(records.length, 0);
    assert.strictEqual(tempDirectories.length, 1);
  });

  it('清理失败可诊断且不覆盖原始 OSS 错误', async () => {
    const original = fs.promises.unlink;
    const cleanupError: any = new Error('测试清理失败');
    cleanupError.code = 'EACCES';
    fs.promises.unlink = async () => { throw cleanupError; };
    try {
      for (const responseStatus of [403, 201]) {
        status = responseStatus;
        await assert.rejects(retryClient().upload(endpoint), (error: any) => {
          if (responseStatus === 403) {
            assert.strictEqual(error.code, 'AccessDenied');
            assert.strictEqual(error.cleanupError, cleanupError);
          } else {
            assert.strictEqual(error, cleanupError);
          }
          return true;
        });
        // 故障注入后由测试使用原始函数清理自己的副本。
        const directory = tempDirectories[tempDirectories.length - 1];
        await original(directory + '/multipart');
        await fs.promises.rmdir(directory);
      }
    } finally {
      fs.promises.unlink = original;
    }
  });

  it('未启用重试及非法地址不建立临时副本', async () => {
    await client.upload(endpoint);
    let reads = 0;
    const content = new Readable({ read() { reads++; this.push(null); } });
    await assert.rejects(retryClient().upload('http://example.com?', new $dara.RuntimeOptions(), content),
      (error: any) => error.code === 'InvalidOSSPrivateUrl');
    assert.strictEqual(reads, 0);
    assert.strictEqual(tempDirectories.length, 0);
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
