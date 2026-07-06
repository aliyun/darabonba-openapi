import * as $dara from '@darabonba/typescript';
import {
  AwapWebSocketHandler,
  ErrUseRawMessage as AwapErrUseRawMessage,
  parseAwapMessage,
} from './awap';
import { WebSocketClient } from './client';
import {
  ErrUseRawMessage as GeneralErrUseRawMessage,
  GeneralWebSocketHandler,
  parseGeneralMessage,
} from './general';

export const SubProtocolAWAP = 'awap';
export const SubProtocolGeneral = 'general';

export class StreamHandler extends $dara.AbstractWebSocketHandler {
  userHandler: $dara.WebSocketHandler;
  subProtocol: string;
  client: WebSocketClient | null = null;

  constructor(userHandler: $dara.WebSocketHandler, subProtocol: string) {
    super();
    this.userHandler = userHandler;
    this.subProtocol = subProtocol;
  }

  async afterConnectionEstablished(session: $dara.WebSocketSessionInfo): Promise<void> {
    await this.userHandler.afterConnectionEstablished(session);
  }

  async handleError(session: $dara.WebSocketSessionInfo, err: Error): Promise<void> {
    await this.userHandler.handleError(session, err);
  }

  async afterConnectionClosed(session: $dara.WebSocketSessionInfo, code: number, reason: string): Promise<void> {
    await this.userHandler.afterConnectionClosed(session, code, reason);
  }

  async handleRawMessage(session: $dara.WebSocketSessionInfo, message: $dara.WebSocketMessage): Promise<void> {
    const subProtocol = (this.subProtocol || '').toLowerCase();
    switch (subProtocol) {
      case SubProtocolAWAP:
        await this.processAwapMessage(session, message);
        break;
      case SubProtocolGeneral:
        await this.processGeneralMessage(session, message);
        break;
      default:
        await this.userHandler.handleRawMessage(session, message);
        break;
    }
  }

  private async processAwapMessage(session: $dara.WebSocketSessionInfo, message: $dara.WebSocketMessage): Promise<void> {
    const awapMsg = parseAwapMessage(message);

    if (awapMsg.type === 'RECONNECT') {
      if (this.client) {
        this.client.reconnectGracefully().catch(() => {
          // ignore reconnect errors
        });
      }
      return;
    }

    const ackID = awapMsg.headers ? awapMsg.headers['ack-id'] : '';
    if (ackID && this.client && this.client.completeRequest(ackID, awapMsg)) {
      return;
    }

    const awapHandler = this.userHandler as AwapWebSocketHandler;
    if (typeof awapHandler.handleAwapMessage === 'function') {
      try {
        await awapHandler.handleAwapMessage(session, awapMsg);
      } catch (err) {
        if (err === AwapErrUseRawMessage) {
          await awapHandler.handleRawMessage(session, message);
        } else {
          await awapHandler.handleError(session, err as Error);
        }
      }
    } else {
      await this.userHandler.handleRawMessage(session, message);
    }
  }

  private async processGeneralMessage(session: $dara.WebSocketSessionInfo, message: $dara.WebSocketMessage): Promise<void> {
    const generalHandler = this.userHandler as GeneralWebSocketHandler;
    if (typeof generalHandler.handleGeneralMessage !== 'function') {
      await this.userHandler.handleRawMessage(session, message);
      return;
    }

    const generalMsg = parseGeneralMessage(message);
    try {
      await generalHandler.handleGeneralMessage(session, generalMsg);
    } catch (err) {
      if (err === GeneralErrUseRawMessage) {
        await generalHandler.handleRawMessage(session, message);
      } else {
        await generalHandler.handleError(session, err as Error);
      }
    }
  }
}
