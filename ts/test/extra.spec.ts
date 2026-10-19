'use strict';

import 'mocha';
import assert from 'assert';

import OpenApiUtil from '../src/utils';
import { AlibabaCloudError } from '../src/exceptions/AlibabaCloudError';
import { ClientError } from '../src/exceptions/ClientError';
import { ServerError } from '../src/exceptions/ServerError';
import { ThrottlingError } from '../src/exceptions/ThrottlingError';
import { SSEResponse } from '../src/models/Sseresponse';
import * as $dara from '@darabonba/typescript';
import * as websocketUtils from '../src/websocketUtils';

describe('extra util and model coverage', function () {
  it('hash applyRetryHeaders and stringify helpers', function () {
    const sha = OpenApiUtil.hash(Buffer.from('test'), 'ACS3-HMAC-SHA256');
    assert.strictEqual(sha.toString('hex'), '9f86d081884c7d659a2feaa0c55ad015a3bf4f1b2b0b822cd15d6c15b0f00a08');
    const rsa = OpenApiUtil.hash(Buffer.from('test'), 'ACS3-RSA-SHA256');
    assert.strictEqual(rsa.toString('hex'), sha.toString('hex'));
    assert.strictEqual(OpenApiUtil.hash(Buffer.from('test'), 'ACS3-SHA256'), undefined);

    const headers: { [key: string]: string } = {};
    OpenApiUtil.applyRetryHeaders(headers, 0, 10);
    assert.strictEqual(headers['x-acs-retry-attempts'], undefined);
    OpenApiUtil.applyRetryHeaders(headers, 2, 15);
    assert.strictEqual(headers['x-acs-retry-attempts'], '2');
    assert.strictEqual(headers['x-acs-retry-delay'], '15');

    assert.ok(OpenApiUtil.getUserAgent('tea').indexOf('tea') >= 0);
    assert.ok(OpenApiUtil.getUserAgent('').indexOf('AlibabaCloud') >= 0);

    const parsed = OpenApiUtil.parseToMap({ k: 'v' });
    assert.strictEqual(parsed['k'], 'v');
    assert.strictEqual(OpenApiUtil.parseToMap(null as any), null);
    const arr = OpenApiUtil.toArray([{ k: 'v' }]);
    assert.strictEqual(arr[0]['k'], 'v');
    assert.strictEqual(OpenApiUtil.toArray(null as any), null);

    const stringified = OpenApiUtil.stringifyMapValue({
      num: 10,
      nil: null,
      str: 'ok',
    });
    assert.strictEqual(stringified['num'], '10');
    assert.strictEqual(stringified['str'], 'ok');

    const endpoint = OpenApiUtil.getEndpointRules('ecs', 'cn-hangzhou', 'regional', 'public', '');
    assert.strictEqual(endpoint, 'ecs.cn-hangzhou.aliyuncs.com');
  });

  it('exceptions and SSEResponse', function () {
    const clientErr = new ClientError({
      statusCode: 400,
      code: 'Client.Error',
      message: 'bad',
      accessDeniedDetail: { k: 'v' },
    });
    assert.strictEqual(clientErr.name, 'ClientError');
    assert.strictEqual(clientErr.code, 'Client.Error');
    assert.deepStrictEqual(clientErr.accessDeniedDetail, { k: 'v' });

    const serverErr = new ServerError({
      statusCode: 500,
      code: 'Internal',
      message: 'boom',
    });
    assert.strictEqual(serverErr.name, 'ServerError');

    const throttling = new ThrottlingError({
      statusCode: 429,
      code: 'Throttling',
      message: 'slow',
      retryAfter: 1000,
    });
    assert.strictEqual(throttling.name, 'ThrottlingError');
    assert.strictEqual(throttling.retryAfter, 1000);

    const cloud = new AlibabaCloudError({
      statusCode: 400,
      code: 'X',
      message: 'y',
      detail: 'd',
      description: 'desc',
      requestId: 'rid',
    });
    assert.strictEqual(cloud.name, 'AlibabaCloudError');
    assert.strictEqual(cloud.requestId, 'rid');

    const sse = new SSEResponse({
      headers: { k: 'v' },
      statusCode: 200,
      event: new $dara.SSEEvent({ data: 'd', id: 'i', event: 'flow' }),
    });
    assert.strictEqual(sse.statusCode, 200);
    sse.validate();
    assert.strictEqual(SSEResponse.names().statusCode, 'statusCode');
    assert.ok(SSEResponse.types().event);
  });

  it('getAuthorization with empty pathname and query', function () {
    const req = new $dara.Request();
    req.method = 'GET';
    req.pathname = '';
    req.query = { empty: '', q: 'a b' };
    req.headers = {
      host: 'example.com',
      'content-type': 'application/json',
      'x-acs-action': 'Test',
    };
    const auth = OpenApiUtil.getAuthorization(req, 'ACS3-HMAC-SHA256', 'payload', 'ak', 'secret');
    assert.ok(auth.indexOf('ACS3-HMAC-SHA256 Credential=ak,') === 0);
  });
});

describe('extra websocket coverage', function () {
  it('client helpers send timeout and parse branches', async function () {
    const texts: string[] = [];
    const binaries: Buffer[] = [];
    const mockWsClient = {
      sendText: async (text: string) => { texts.push(text); },
      sendBinary: async (data: Buffer) => { binaries.push(data); },
      isConnected: () => true,
      close: async () => { /* noop */ },
      reconnect: async () => new $dara.Response(<any>{ statusCode: 101, headers: {} }),
      reconnectGracefully: async () => new $dara.Response(<any>{ statusCode: 101, headers: {} }),
      disconnect: async () => { /* noop */ },
      getSessionInfo: () => null,
    };
    const client = new websocketUtils.WebSocketClient(
      mockWsClient as any,
      new $dara.Response(<any>{ statusCode: 101, headers: {} }),
    );
    assert.strictEqual(websocketUtils.WebSocketClient.createWebSocketClient(null), null);
    assert.strictEqual(websocketUtils.WebSocketClient.createWebSocketClient('nope'), null);
    assert.strictEqual(websocketUtils.WebSocketClient.createWebSocketClient(client), client);
    client.validate();
    assert.strictEqual(client.isConnected(), true);
    assert.strictEqual(client.getSessionInfo(), null);
    await client.sendRawAwapTextMessage('UpstreamTextEvent', { a: 'b' });
    await client.sendRawAwapTextMessageWithId('UpstreamTextEvent', 'id-1', { a: 'b' });
    await client.sendRawAwapBinaryMessage('UpstreamBinaryEvent', Buffer.from('bin'));
    await client.sendRawAwapBinaryMessageWithId('UpstreamBinaryEvent', 'id-2', Buffer.from('bin2'));
    await client.sendGeneralTextMessage('hello');
    await client.sendGeneralBinaryMessage(Buffer.from('bin'));
    await client.reconnect();
    await client.reconnectGracefully();
    await client.disconnect();
    await client.close();
    assert.ok(texts.length >= 3);
    assert.ok(binaries.length >= 3);

    await assert.rejects(() => client.sendRequest('', 'text', 1), /cannot be empty/);
    await assert.rejects(() => client.sendRequest('id', '', 1), /cannot be empty/);
    await assert.rejects(() => client.sendRequest('missing', 'payload', 5), /timeout/);

    const nonePayload = websocketUtils.newAwapMessage(undefined as any, websocketUtils.withType('UpstreamTextEvent'));
    assert.ok(websocketUtils.buildAwapMessageText(nonePayload).indexOf('{}') >= 0);
    assert.throws(() => websocketUtils.buildAwapMessageText(null as any));
    assert.throws(() => websocketUtils.buildAwapMessageBinary(null as any));
    assert.throws(() => websocketUtils.buildAwapMessageBinary({ type: 'UpstreamBinaryEvent', payload: 'not-bytes' } as any));
    const emptyBin = websocketUtils.buildAwapMessageBinary({ type: 'UpstreamBinaryEvent' } as any);
    assert.ok(emptyBin.length > 0);

    const parsedText = websocketUtils.parseGeneralMessage({
      type: $dara.WebSocketMessageType.Text,
      payload: Buffer.from('not-json'),
      headers: {},
      timestamp: new Date(),
    });
    assert.strictEqual(parsedText.format, 'text');
  });
});
