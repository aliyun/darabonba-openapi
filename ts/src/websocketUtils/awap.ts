import * as $dara from '@darabonba/typescript';

export type AwapMessageType = string;
export type AwapMessageFormat = 'text' | 'binary';

export interface AwapMessage {
  type: AwapMessageType;
  id: string;
  headers?: { [key: string]: string };
  payload?: any;
  format?: AwapMessageFormat;
}

export interface AwapWebSocketHandler extends $dara.WebSocketHandler {
  handleAwapMessage(session: $dara.WebSocketSessionInfo, message: AwapMessage): Promise<void> | void;
}

export class AbstractAwapWebSocketHandler extends $dara.AbstractWebSocketHandler implements AwapWebSocketHandler {
  handleAwapMessage(_session: $dara.WebSocketSessionInfo, _message: AwapMessage): void {
    throw ErrUseRawMessage;
  }
}

export type AwapMessageOption = (message: AwapMessage) => void;

export function withType(msgType: AwapMessageType): AwapMessageOption {
  return (message: AwapMessage) => {
    message.type = msgType;
  };
}

export function withID(id: string): AwapMessageOption {
  return (message: AwapMessage) => {
    message.id = id;
  };
}

export function withHeader(key: string, value: string): AwapMessageOption {
  return (message: AwapMessage) => {
    message.headers = message.headers || {};
    message.headers[key] = value;
  };
}

export function newAwapMessage(payload: any, ...opts: AwapMessageOption[]): AwapMessage {
  const message: AwapMessage = {
    type: 'UpstreamTextEvent',
    id: generateMessageId(),
    payload,
    headers: {},
  };
  opts.forEach((opt) => opt(message));
  return message;
}

export function buildAwapMessageText(message: AwapMessage): string {
  if (!message) {
    throw new Error('message cannot be nil');
  }

  const now = Date.now();
  const headerLines: string[] = [
    `type:${message.type}`,
    `timestamp:${now}`,
  ];

  if (message.id) {
    headerLines.push(`id:${message.id}`);
  }

  if (message.type === 'AckRequiredTextEvent') {
    headerLines.push('ack:required');
  }

  if (message.headers) {
    Object.keys(message.headers).forEach((key) => {
      headerLines.push(`${key}:${message.headers![key]}`);
    });
  }

  let payloadJSON = '{}';
  if (message.payload !== undefined && message.payload !== null) {
    payloadJSON = JSON.stringify(message.payload);
  }

  return `${headerLines.join('\n')}\n\n${payloadJSON}`;
}

export function buildAwapMessageBinary(message: AwapMessage): Buffer {
  if (!message) {
    throw new Error('message cannot be nil');
  }

  const now = Date.now();
  const headerLines: string[] = [
    `type:${message.type}`,
    `timestamp:${now}`,
  ];

  if (message.id) {
    headerLines.push(`id:${message.id}`);
  }

  if (message.headers) {
    Object.keys(message.headers).forEach((key) => {
      headerLines.push(`${key}:${message.headers![key]}`);
    });
  }

  const headerBytes = Buffer.from(`${headerLines.join('\n')}\n\n`, 'utf8');
  let bodyBytes: Buffer;
  if (message.payload !== undefined && message.payload !== null) {
    if (!Buffer.isBuffer(message.payload)) {
      throw new Error(`payload for binary AWAP message must be Buffer, got ${typeof message.payload}`);
    }
    bodyBytes = message.payload;
  } else {
    bodyBytes = Buffer.alloc(0);
  }

  return Buffer.concat([headerBytes, bodyBytes]);
}

export function parseAwapMessage(message: $dara.WebSocketMessage): AwapMessage {
  const data = message.payload;
  let headerEndIndex = -1;
  for (let i = 0; i < data.length - 1; i++) {
    if (data[i] === 0x0a && data[i + 1] === 0x0a) {
      headerEndIndex = i;
      break;
    }
  }

  if (headerEndIndex === -1) {
    throw new Error('failed to parse AWAP message: no \\n\\n separator found');
  }

  const awapMsg: AwapMessage = {
    type: '',
    id: '',
    headers: {},
  };

  const headerStr = data.slice(0, headerEndIndex).toString('utf8');
  const payloadBytes = data.slice(headerEndIndex + 2);

  headerStr.split('\n').forEach((line) => {
    const trimmed = line.trim();
    if (!trimmed) {
      return;
    }
    const colonIndex = trimmed.indexOf(':');
    if (colonIndex > 0) {
      const key = trimmed.slice(0, colonIndex).trim();
      const value = trimmed.slice(colonIndex + 1).trim();
      switch (key) {
        case 'type':
          awapMsg.type = value;
          break;
        case 'id':
          awapMsg.id = value;
          break;
        default:
          awapMsg.headers![key] = value;
          break;
      }
    }
  });

  if (payloadBytes.length > 0) {
    try {
      awapMsg.payload = JSON.parse(payloadBytes.toString('utf8'));
    } catch (_err) {
      awapMsg.payload = payloadBytes;
    }
  }

  awapMsg.format = message.type === $dara.WebSocketMessageType.Binary ? 'binary' : 'text';
  return awapMsg;
}

function generateMessageId(): string {
  const crypto = require('crypto');
  const hexStr = crypto.randomBytes(16).toString('hex');
  if (hexStr.length >= 32) {
    return hexStr.slice(0, 32);
  }
  const timestamp = `${Date.now()}`;
  const combined = timestamp + hexStr;
  if (combined.length >= 32) {
    return combined.slice(0, 32);
  }
  return combined.padEnd(32, '0');
}

export const ErrUseRawMessage = new Error('use HandleRawMessage');
