import * as $dara from '@darabonba/typescript';
import { ErrUseRawMessage } from './awap';

export type GeneralMessageFormat = 'text' | 'binary';

export interface GeneralMessage {
  body?: any;
  format?: GeneralMessageFormat;
}

export interface GeneralWebSocketHandler extends $dara.WebSocketHandler {
  handleGeneralMessage(session: $dara.WebSocketSessionInfo, message: GeneralMessage): Promise<void> | void;
}

export class AbstractGeneralWebSocketHandler extends $dara.AbstractWebSocketHandler implements GeneralWebSocketHandler {
  handleGeneralMessage(_session: $dara.WebSocketSessionInfo, _message: GeneralMessage): void {
    throw ErrUseRawMessage;
  }
}

export function newGeneralMessage(body: string): GeneralMessage {
  return { body };
}

export function parseGeneralMessage(message: $dara.WebSocketMessage): GeneralMessage {
  if (message.type === $dara.WebSocketMessageType.Binary) {
    return {
      body: message.payload,
      format: 'binary',
    };
  }

  try {
    return {
      body: JSON.parse(message.payload.toString('utf8')),
      format: 'text',
    };
  } catch (_err) {
    return {
      body: message.payload,
      format: 'text',
    };
  }
}

export function generalMessageToJSON(message: GeneralMessage): Buffer {
  return Buffer.from(JSON.stringify(message));
}

export { ErrUseRawMessage };
