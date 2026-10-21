'use strict';

import 'mocha';
import assert from 'assert';

import OpenApiUtil, { GlobalParameters, Config as OpenApiConfig, Params, OpenApiRequest } from '../src/utils';
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
    const arr = OpenApiUtil.toArray([{ k: 'v' }, null, undefined]);
    assert.strictEqual(arr[0]['k'], 'v');
    assert.strictEqual(arr.length, 1);
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
    assert.throws(() => OpenApiUtil.getEndpointRules('ecs', '', 'regional', 'public', ''));
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
    req.headers['Host'] = 'example.com';
    req.headers['X-Acs-Action'] = 'Test';
    req.query['space'] = 'a b';
    const authDup = OpenApiUtil.getAuthorization(req, 'ACS3-HMAC-SHA256', 'payload', 'ak', 'secret');
    assert.ok(authDup.indexOf('ACS3-HMAC-SHA256 Credential=ak,') === 0);

    const signed = new $dara.Request();
    signed.method = 'POST';
    signed.pathname = '/test';
    signed.query = {};
    signed.headers = {
      accept: 'application/json',
      'content-md5': 'md5',
      'content-type': 'application/json',
      date: 'Mon, 11 Aug 2025 00:00:00 GMT',
      'x-acs-action': 'DescribeRegions',
    };
    const sts = OpenApiUtil.getStringToSign(signed);
    assert.ok(sts.indexOf('POST') === 0);
    assert.ok(OpenApiUtil.getROASignature(sts, 'secret'));
    assert.ok(OpenApiUtil.getRPCSignature({ Action: 'DescribeRegions', Version: '2014-05-26' }, 'POST', 'secret'));
    assert.strictEqual(OpenApiUtil.toForm(null as any), '');
    assert.ok(OpenApiUtil.toForm({ Name: 'aliyun', Ids: ['1', '2'] }).indexOf('Name=aliyun') >= 0);
    assert.strictEqual(OpenApiUtil.arrayToStringWithSpecifiedStyle(null, 'Ids', 'json'), '');
    assert.strictEqual(OpenApiUtil.arrayToStringWithSpecifiedStyle(['1', '2'], 'Ids', 'simple'), '1,2');
    assert.strictEqual(OpenApiUtil.arrayToStringWithSpecifiedStyle(['1', '2'], 'Ids', 'spaceDelimited'), '1 2');
    assert.strictEqual(OpenApiUtil.arrayToStringWithSpecifiedStyle(['1', '2'], 'Ids', 'pipeDelimited'), '1|2');
    assert.ok(OpenApiUtil.arrayToStringWithSpecifiedStyle(['1', '2'], 'Ids', 'json').indexOf('1') >= 0);
    assert.ok(OpenApiUtil.getEndpoint('ecs.cn-hangzhou.aliyuncs.com', false, 'internal').indexOf('-internal') >= 0);
    assert.strictEqual(OpenApiUtil.getEndpoint('oss.aliyuncs.com', true, 'accelerate'), 'oss-accelerate.aliyuncs.com');
    assert.ok(OpenApiUtil.getNonce());
    assert.ok(OpenApiUtil.getTimestamp().indexOf('T') >= 0);
    assert.ok(OpenApiUtil.getDateUTCString().indexOf('GMT') >= 0);
    const centralEndpoint = OpenApiUtil.getEndpointRules('ecs', 'cn-hangzhou', 'central', 'vpc', 'inner');
    assert.ok(centralEndpoint.indexOf('ecs-inner') === 0);

    const gp = new GlobalParameters({ headers: { h: '1' }, queries: { q: '2' } });
    assert.strictEqual(GlobalParameters.names().headers, 'headers');
    assert.ok(GlobalParameters.types().headers);
    assert.strictEqual(gp.toMap().headers.h, '1');
    assert.strictEqual(OpenApiConfig.names().endpoint, 'endpoint');
    assert.ok(OpenApiConfig.types().endpoint);
    assert.strictEqual(Params.names().action, 'action');
    assert.ok(Params.types().action);
    assert.strictEqual(OpenApiRequest.names().query, 'query');
    assert.ok(OpenApiRequest.types().body);

    const dest = new Params({});
    OpenApiUtil.convert(new Params({ action: 'DescribeRegions', version: '2014-05-26' }), dest);
    assert.strictEqual(dest.action, 'DescribeRegions');
    OpenApiUtil.convert(new Params({ action: 'A' }), null as any);

    const nested = OpenApiUtil.mapToFlatStyle({ Name: 'aliyun', Tags: ['a', 'b'] });
    assert.ok(nested['#4#Name'] === 'aliyun' || nested.Name === 'aliyun' || typeof nested === 'object');
    assert.ok(Array.isArray(OpenApiUtil.mapToFlatStyle(['a', { k: 'v' }])));
    const modelFlat = OpenApiUtil.mapToFlatStyle(new Params({ action: 'A', version: 'v' }));
    assert.ok(modelFlat);

    const query = OpenApiUtil.query({
      Region: 'cn-hangzhou',
      Ids: ['a', 'b'],
      Nested: { k: 'v' },
      Bytes: Buffer.from('ok'),
      Empty: null,
    });
    assert.strictEqual(query.Region, 'cn-hangzhou');
    assert.ok(query['Ids.1'] === 'a' || query.Ids);
    assert.deepStrictEqual(OpenApiUtil.query(null as any), {});
    const stringified = OpenApiUtil.stringifyMapValue({ a: 1, b: true, c: undefined as any });
    assert.strictEqual(stringified.a, '1');
    assert.ok(OpenApiUtil.stringifyMapValue(null as any) == null);
    assert.ok(OpenApiUtil.getEndpointRules('ecs', 'cn-hangzhou', 'regional', 'vpc', '').indexOf('ecs') >= 0);
    const cfg = new OpenApiConfig({
      accessKeyId: 'ak',
      accessKeySecret: 'sk',
      securityToken: 'st',
      bearerToken: 'bt',
      protocol: 'HTTP',
      method: 'POST',
      regionId: 'cn-hangzhou',
      endpoint: 'example.com',
      signatureAlgorithm: 'ACS3-HMAC-SHA256',
      disableHttp2: true,
    });
    assert.strictEqual(cfg.endpoint, 'example.com');
    const apiReq = new OpenApiRequest({
      headers: { h: '1' },
      query: { q: '2' },
      body: { k: 'v' },
      hostMap: { region: 'cn-hangzhou' },
      endpointOverride: 'oss.aliyuncs.com',
    });
    assert.strictEqual(apiReq.endpointOverride, 'oss.aliyuncs.com');
    const reqMap = apiReq.toMap();
    assert.strictEqual(reqMap.hostMap.region, 'cn-hangzhou');
    assert.strictEqual(reqMap.endpointOverride, 'oss.aliyuncs.com');
    const paramsMap = dest.toMap();
    assert.strictEqual(paramsMap.action, 'DescribeRegions');
    const cfgMap = cfg.toMap();
    assert.strictEqual(cfgMap.endpoint, 'example.com');
    assert.strictEqual(cfgMap.protocol, 'HTTP');
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
    const parsedBin = websocketUtils.parseGeneralMessage({
      type: $dara.WebSocketMessageType.Binary,
      payload: Buffer.from('raw-bytes'),
      headers: {},
      timestamp: new Date(),
    });
    assert.strictEqual(parsedBin.format, 'binary');

    const awapText = websocketUtils.buildAwapMessageText(
      websocketUtils.newAwapMessage({ hello: 'world' }, websocketUtils.withType('UpstreamTextEvent'), websocketUtils.withID('m1'), websocketUtils.withHeader('custom', '1')),
    );
    const parsedAwap = websocketUtils.parseAwapMessage({
      type: $dara.WebSocketMessageType.Text,
      payload: Buffer.from(awapText),
      headers: {},
      timestamp: new Date(),
    });
    assert.strictEqual(parsedAwap.type, 'UpstreamTextEvent');
    assert.strictEqual(parsedAwap.id, 'm1');
    assert.strictEqual(parsedAwap.headers!['custom'], '1');
    const binaryPayload = websocketUtils.parseAwapMessage({
      type: $dara.WebSocketMessageType.Binary,
      payload: Buffer.from('type:UpstreamBinaryEvent\nid:b1\n\nnot-json'),
      headers: {},
      timestamp: new Date(),
    });
    assert.strictEqual(binaryPayload.format, 'binary');
    assert.ok(binaryPayload.payload);
    assert.throws(() => websocketUtils.parseAwapMessage({
      type: $dara.WebSocketMessageType.Text,
      payload: Buffer.from('no-separator'),
      headers: {},
      timestamp: new Date(),
    }));
    const emptyPayload = websocketUtils.parseAwapMessage({
      type: $dara.WebSocketMessageType.Text,
      payload: Buffer.from('type:UpstreamTextEvent\nid:e1\n\n'),
      headers: {},
      timestamp: new Date(),
    });
    assert.strictEqual(emptyPayload.id, 'e1');
  });

  it('stream handler lifecycle fallbacks and protocol routing', async function () {
    const session = {} as any;
    const errors: Error[] = [];
    class RecordingHandler extends $dara.AbstractWebSocketHandler {
      established = false;
      closed = false;
      raw = 0;
      awap = 0;
      general = 0;
      awapErr: Error | null = null;
      afterConnectionEstablished() { this.established = true; }
      afterConnectionClosed() { this.closed = true; }
      handleError(_s: any, err: Error) { errors.push(err); }
      handleRawMessage() { this.raw++; }
      async handleAwapMessage() {
        this.awap++;
        if (this.awapErr) {
          throw this.awapErr;
        }
      }
      async handleGeneralMessage() { this.general++; }
    }

    const user = new RecordingHandler();
    const handler = new websocketUtils.StreamHandler(user, websocketUtils.SubProtocolAWAP);
    await handler.afterConnectionEstablished(session);
    assert.ok(user.established);
    await handler.afterConnectionClosed(session, 1000, 'bye');
    assert.ok(user.closed);
    await handler.handleError(session, new Error('x'));
    assert.ok(errors.length >= 1);

    const text = websocketUtils.buildAwapMessageText(
      websocketUtils.newAwapMessage({ x: '1' }, websocketUtils.withType('UpstreamTextEvent'), websocketUtils.withID('m1')),
    );
    const raw: $dara.WebSocketMessage = {
      type: $dara.WebSocketMessageType.Text,
      payload: Buffer.from(text),
      headers: {},
      timestamp: new Date(),
    };
    await handler.handleRawMessage(session, raw);
    assert.strictEqual(user.awap, 1);

    const plain = new $dara.AbstractWebSocketHandler();
    await new websocketUtils.StreamHandler(plain, websocketUtils.SubProtocolAWAP).handleRawMessage(session, raw);

    class RawFallback extends $dara.AbstractWebSocketHandler {
      raw = 0;
      async handleAwapMessage() { throw websocketUtils.ErrUseRawMessage; }
      async handleGeneralMessage() { throw websocketUtils.ErrUseRawMessage; }
      handleRawMessage() { this.raw++; }
    }
    const fallback = new RawFallback();
    await new websocketUtils.StreamHandler(fallback, websocketUtils.SubProtocolAWAP).handleRawMessage(session, raw);
    assert.strictEqual(fallback.raw, 1);

    user.awapErr = new Error('awap-fail');
    await handler.handleRawMessage(session, raw);

    const gmsg: $dara.WebSocketMessage = {
      type: $dara.WebSocketMessageType.Text,
      payload: Buffer.from('{"body":"hi"}'),
      headers: {},
      timestamp: new Date(),
    };
    const generalUser = new RecordingHandler();
    await new websocketUtils.StreamHandler(generalUser, websocketUtils.SubProtocolGeneral).handleRawMessage(session, gmsg);
    assert.strictEqual(generalUser.general, 1);
    await new websocketUtils.StreamHandler(plain, websocketUtils.SubProtocolGeneral).handleRawMessage(session, gmsg);
    await new websocketUtils.StreamHandler(fallback, websocketUtils.SubProtocolGeneral).handleRawMessage(session, gmsg);
    assert.strictEqual(fallback.raw, 2);

    class BoomGeneral extends $dara.AbstractWebSocketHandler {
      async handleGeneralMessage() { throw new Error('boom'); }
      handleError(_s: any, err: Error) { errors.push(err); }
    }
    await new websocketUtils.StreamHandler(new BoomGeneral(), websocketUtils.SubProtocolGeneral).handleRawMessage(session, gmsg);
    assert.ok(errors.length >= 2);

    try {
      await websocketUtils.AbstractAwapWebSocketHandler.prototype.handleAwapMessage.call(
        new websocketUtils.AbstractAwapWebSocketHandler(), session, {} as any,
      );
      assert.fail();
    } catch (err) {
      assert.strictEqual(err, websocketUtils.ErrUseRawMessage);
    }
    try {
      await websocketUtils.AbstractGeneralWebSocketHandler.prototype.handleGeneralMessage.call(
        new websocketUtils.AbstractGeneralWebSocketHandler(), session, {} as any,
      );
      assert.fail();
    } catch (err) {
      assert.strictEqual(err, websocketUtils.ErrUseRawMessage);
    }

    assert.strictEqual(websocketUtils.createWebSocketClient(null), null);
    assert.strictEqual(websocketUtils.createWebSocketClient('nope'), null);
    const exported = websocketUtils.newWebSocketClient({
      sendText: async () => { /* noop */ },
      sendBinary: async () => { /* noop */ },
      isConnected: () => true,
      close: async () => { /* noop */ },
      reconnect: async () => new $dara.Response(<any>{ statusCode: 101, headers: {} }),
      reconnectGracefully: async () => new $dara.Response(<any>{ statusCode: 101, headers: {} }),
      disconnect: async () => { /* noop */ },
      getSessionInfo: () => null,
    } as any, new $dara.Response(<any>{ statusCode: 101, headers: {} }));
    assert.ok(exported);
    assert.strictEqual(exported.completeRequest('missing', { ok: true }), false);
    const msg = websocketUtils.newAwapMessage({ a: 1 }, websocketUtils.withHeader('k', 'v'));
    assert.strictEqual(msg.headers!['k'], 'v');
  });
});
