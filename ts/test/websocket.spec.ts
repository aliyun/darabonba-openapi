'use strict';

import 'mocha';
import assert from 'assert';
import * as $dara from '@darabonba/typescript';
import * as websocketUtils from '../src/websocketUtils';
import Client from '../src/client';
import { Params, OpenApiRequest, Config } from '../src/utils';

class MockWebSocketHandler extends $dara.AbstractWebSocketHandler {
  connectedCalled = false;

  afterConnectionEstablished(_session: $dara.WebSocketSessionInfo): void {
    this.connectedCalled = true;
  }
}

describe('websocketUtils', function () {
  it('should build and parse AWAP text messages', function () {
    const message = websocketUtils.newAwapMessage(
      { hello: 'world' },
      websocketUtils.withType('UpstreamTextEvent'),
      websocketUtils.withID('msg-001'),
      websocketUtils.withHeader('custom', 'value'),
    );

    const text = websocketUtils.buildAwapMessageText(message);
    assert.ok(text.includes('type:UpstreamTextEvent'));
    assert.ok(text.includes('id:msg-001'));
    assert.ok(text.includes('custom:value'));
    assert.ok(text.endsWith('{"hello":"world"}'));

    const rawMessage: $dara.WebSocketMessage = {
      type: $dara.WebSocketMessageType.Text,
      payload: Buffer.from(text),
      headers: {},
      timestamp: new Date(),
    };
    const parsed = websocketUtils.parseAwapMessage(rawMessage);
    assert.strictEqual(parsed.type, 'UpstreamTextEvent');
    assert.strictEqual(parsed.id, 'msg-001');
    assert.deepStrictEqual(parsed.payload, { hello: 'world' });
    assert.strictEqual(parsed.headers!.custom, 'value');
  });

  it('should add ack:required for AckRequiredTextEvent', function () {
    const message = websocketUtils.newAwapMessage({}, websocketUtils.withType('AckRequiredTextEvent'));
    const text = websocketUtils.buildAwapMessageText(message);
    assert.ok(text.includes('ack:required'));
  });

  it('should parse general messages', function () {
    const jsonMessage: $dara.WebSocketMessage = {
      type: $dara.WebSocketMessageType.Text,
      payload: Buffer.from(JSON.stringify({ body: 'hello' })),
      headers: {},
      timestamp: new Date(),
    };
    const parsedJson = websocketUtils.parseGeneralMessage(jsonMessage);
    assert.deepStrictEqual(parsedJson.body, { body: 'hello' });
    assert.strictEqual(parsedJson.format, 'text');

    const binaryMessage: $dara.WebSocketMessage = {
      type: $dara.WebSocketMessageType.Binary,
      payload: Buffer.from('binary-data'),
      headers: {},
      timestamp: new Date(),
    };
    const parsedBinary = websocketUtils.parseGeneralMessage(binaryMessage);
    assert.strictEqual(parsedBinary.body.toString(), 'binary-data');
    assert.strictEqual(parsedBinary.format, 'binary');
  });

  it('should complete AWAP request-response pattern', async function () {
    const mockWsClient = {
      sendText: async (_text: string) => { /* noop */ },
      isConnected: () => true,
      close: async () => { /* noop */ },
      reconnect: async () => { throw new Error('not implemented'); },
      reconnectGracefully: async () => { throw new Error('not implemented'); },
      disconnect: async () => { /* noop */ },
      getSessionInfo: () => null,
    };

    const client = new websocketUtils.WebSocketClient(
      mockWsClient as any,
      new $dara.Response(<any>{ statusCode: 101, statusMessage: 'Switching Protocols', headers: {} }),
    );

    const responsePromise = client.sendRequest('ack-001', 'type:test\nid:ack-001\n\n{}', 1000);
    assert.strictEqual(client.completeRequest('ack-001', { ok: true }), true);
    const response = await responsePromise;
    assert.deepStrictEqual(response, { ok: true });
  });

  it('should route AWAP RECONNECT messages to graceful reconnect', async function () {
    let gracefulCalled = false;
    const mockWsClient = {
      reconnectGracefully: async () => {
        gracefulCalled = true;
        return new $dara.Response(<any>{ statusCode: 101, statusMessage: 'Switching Protocols', headers: {} });
      },
    };
    const wsClient = new websocketUtils.WebSocketClient(mockWsClient as any, new $dara.Response(<any>{ statusCode: 101, headers: {} }));
    const handler = new websocketUtils.StreamHandler(new MockWebSocketHandler(), websocketUtils.SubProtocolAWAP);
    handler.client = wsClient;

    const awapText = websocketUtils.buildAwapMessageText(
      websocketUtils.newAwapMessage({}, websocketUtils.withType('RECONNECT')),
    );
    await handler.handleRawMessage(
      {
        sessionID: 's1',
        requestID: '',
        connectedAt: new Date(),
        remoteAddr: '',
        localAddr: '',
        attributes: {},
      },
      {
        type: $dara.WebSocketMessageType.Text,
        payload: Buffer.from(awapText),
        headers: {},
        timestamp: new Date(),
      },
    );

    await $dara.sleep(20);
    assert.strictEqual(gracefulCalled, true);
  });

  it('should reject unknown sub protocol', async function () {
    const handler = new websocketUtils.StreamHandler(new MockWebSocketHandler(), 'invalid');
    await assert.rejects(
      () => handler.handleRawMessage(
        {
          sessionID: 's1',
          requestID: '',
          connectedAt: new Date(),
          remoteAddr: '',
          localAddr: '',
          attributes: {},
        },
        {
          type: $dara.WebSocketMessageType.Text,
          payload: Buffer.from('{}'),
          headers: {},
          timestamp: new Date(),
        },
      ),
      /unsupported websocketSubProtocol/,
    );
  });

  it('should call handleError when reconnect fails', async function () {
    const errors: Error[] = [];
    class ErrorHandler extends MockWebSocketHandler {
      async handleError(_session: $dara.WebSocketSessionInfo, err: Error): Promise<void> {
        errors.push(err);
      }
    }

    const mockWsClient = {
      reconnectGracefully: async () => {
        throw new Error('reconnect failed');
      },
    };
    const wsClient = new websocketUtils.WebSocketClient(
      mockWsClient as any,
      new $dara.Response(<any>{ statusCode: 101, headers: {} }),
    );
    const handler = new websocketUtils.StreamHandler(new ErrorHandler(), websocketUtils.SubProtocolAWAP);
    handler.client = wsClient;

    const awapText = websocketUtils.buildAwapMessageText(
      websocketUtils.newAwapMessage({}, websocketUtils.withType('RECONNECT')),
    );
    await handler.handleRawMessage(
      {
        sessionID: 's1',
        requestID: '',
        connectedAt: new Date(),
        remoteAddr: '',
        localAddr: '',
        attributes: {},
      },
      {
        type: $dara.WebSocketMessageType.Text,
        payload: Buffer.from(awapText),
        headers: {},
        timestamp: new Date(),
      },
    );

    assert.strictEqual(errors.length, 1);
    assert.ok(/reconnect failed/.test(errors[0].message));
  });
});

describe('Client.doRequest websocket', function () {
  it('should require websocketSubProtocol for ws protocol', async function () {
    const client = new Client(new Config({
      accessKeyId: 'ak',
      accessKeySecret: 'sk',
      endpoint: 'example.com',
      protocol: 'ws',
    }));

    const params = new Params({
      action: 'TestAction',
      version: '2020-01-01',
      protocol: 'ws',
      pathname: '/ws',
      method: 'GET',
      authType: 'AK',
      bodyType: 'string',
      reqBodyType: 'json',
    });

    await assert.rejects(
      () => client.doRequest(params, new OpenApiRequest({}), new $dara.RuntimeOptions({
        webSocketHandler: new MockWebSocketHandler(),
      })),
      /websocketSubProtocol is required/,
    );
  });

  it('should require WebSocketHandler for ws protocol', async function () {
    const client = new Client(new Config({
      accessKeyId: 'ak',
      accessKeySecret: 'sk',
      endpoint: 'example.com',
      protocol: 'ws',
    }));

    const params = new Params({
      action: 'TestAction',
      version: '2020-01-01',
      protocol: 'ws',
      pathname: '/ws',
      method: 'GET',
      authType: 'AK',
      bodyType: 'string',
      reqBodyType: 'json',
      websocketSubProtocol: websocketUtils.SubProtocolAWAP,
    });

    await assert.rejects(
      () => client.doRequest(params, new OpenApiRequest({}), new $dara.RuntimeOptions({})),
      /WebSocketHandler is required/,
    );
  });

  it('should reject invalid websocketSubProtocol values', async function () {
    const client = new Client(new Config({
      accessKeyId: 'ak',
      accessKeySecret: 'sk',
      endpoint: 'example.com',
      protocol: 'ws',
    }));

    const params = new Params({
      action: 'TestAction',
      version: '2020-01-01',
      protocol: 'ws',
      pathname: '/ws',
      method: 'GET',
      authType: 'AK',
      bodyType: 'string',
      reqBodyType: 'json',
      websocketSubProtocol: 'invalid',
    });

    await assert.rejects(
      () => client.doRequest(params, new OpenApiRequest({}), new $dara.RuntimeOptions({
        webSocketHandler: new MockWebSocketHandler(),
      })),
      /websocketSubProtocol must be 'awap' or 'general'/,
    );
  });
});
