import * as $dara from '@darabonba/typescript';
import {
  AwapMessage,
  AwapMessageType,
  buildAwapMessageBinary,
  buildAwapMessageText,
  newAwapMessage,
  withID,
  withType,
} from './awap';
import { GeneralMessage, generalMessageToJSON, newGeneralMessage } from './general';

export class WebSocketClient {
  private wsClient: $dara.DefaultWebSocketClient;
  private response: $dara.Response;
  private pendingRequests: { [key: string]: Array<(value: any) => void> } = {};

  constructor(wsClient: $dara.DefaultWebSocketClient, response: $dara.Response) {
    this.wsClient = wsClient;
    this.response = response;
  }

  static createWebSocketClient(client: any): WebSocketClient | null {
    if (!client) {
      return null;
    }
    if (client instanceof WebSocketClient) {
      return client;
    }
    return null;
  }

  getResponse(): $dara.Response {
    return this.response;
  }

  async close(): Promise<void> {
    if (this.wsClient) {
      await this.wsClient.close();
    }
  }

  async reconnect(): Promise<$dara.Response> {
    return this.wsClient.reconnect();
  }

  async reconnectGracefully(): Promise<$dara.Response> {
    return this.wsClient.reconnectGracefully();
  }

  async disconnect(): Promise<void> {
    await this.wsClient.disconnect();
  }

  isConnected(): boolean {
    return this.wsClient.isConnected();
  }

  validate(): void {
    if (!this.wsClient) {
      throw new Error('failed to build websocket client');
    }
  }

  getSessionInfo(): $dara.WebSocketSessionInfo | null {
    return this.wsClient.getSessionInfo();
  }

  async sendAwapTextMessage(message: AwapMessage): Promise<void> {
    const messageText = buildAwapMessageText(message);
    await this.wsClient.sendText(messageText);
  }

  async sendRawAwapTextMessage(msgType: AwapMessageType, payload: any): Promise<void> {
    const message = newAwapMessage(payload, withType(msgType));
    await this.sendAwapTextMessage(message);
  }

  async sendRawAwapTextMessageWithId(msgType: AwapMessageType, id: string, payload: any): Promise<void> {
    const message = newAwapMessage(payload, withType(msgType), withID(id));
    await this.sendAwapTextMessage(message);
  }

  async sendAwapBinaryMessage(message: AwapMessage): Promise<void> {
    const messageBinary = buildAwapMessageBinary(message);
    await this.wsClient.sendBinary(messageBinary);
  }

  async sendRawAwapBinaryMessage(msgType: AwapMessageType, payload: any): Promise<void> {
    const message = newAwapMessage(payload, withType(msgType));
    await this.sendAwapBinaryMessage(message);
  }

  async sendRawAwapBinaryMessageWithId(msgType: AwapMessageType, id: string, payload: any): Promise<void> {
    const message = newAwapMessage(payload, withType(msgType), withID(id));
    await this.sendAwapBinaryMessage(message);
  }

  async sendAwapRequestWithAck(message: AwapMessage, timeoutMs: number): Promise<any> {
    const messageText = buildAwapMessageText(message);
    return this.sendRequest(message.id, messageText, timeoutMs);
  }

  async sendRequest(ackID: string, messageText: string, timeoutMs: number): Promise<any> {
    if (!ackID) {
      throw new Error('message ID cannot be empty for request-response pattern');
    }
    if (!messageText) {
      throw new Error('message text cannot be empty for request-response pattern');
    }

    return new Promise<any>((resolve, reject) => {
      this.pendingRequests[ackID] = this.pendingRequests[ackID] || [];
      this.pendingRequests[ackID].push(resolve);

      this.wsClient.sendText(messageText).catch((err) => {
        delete this.pendingRequests[ackID];
        reject(err);
      });

      const timeout = timeoutMs > 0 ? timeoutMs : 30000;
      setTimeout(() => {
        if (this.pendingRequests[ackID]) {
          delete this.pendingRequests[ackID];
          reject(new Error(`request timeout after ${timeout}ms waiting for response to message ID: ${ackID}`));
        }
      }, timeout);
    });
  }

  completeRequest(ackID: string, response: any): boolean {
    const waiters = this.pendingRequests[ackID];
    if (!waiters || waiters.length === 0) {
      return false;
    }
    delete this.pendingRequests[ackID];
    waiters.forEach((resolve) => resolve(response));
    return true;
  }

  async sendGeneralTextMessage(text: string): Promise<void> {
    const message = newGeneralMessage(text);
    const jsonData = generalMessageToJSON(message);
    await this.wsClient.sendText(jsonData.toString('utf8'));
  }

  async sendGeneralBinaryMessage(data: Buffer): Promise<void> {
    await this.wsClient.sendBinary(data);
  }
}

export function newWebSocketClient(wsClient: $dara.DefaultWebSocketClient, response: $dara.Response): WebSocketClient {
  return new WebSocketClient(wsClient, response);
}

export type { AwapMessage, GeneralMessage };
