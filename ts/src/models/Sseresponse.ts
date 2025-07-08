// This file is auto-generated, don't edit it
import * as $dara from '@darabonba/typescript';


/**
 * @remarks
 * This is for OpenApi SDK
 */
export class SSEResponse extends $dara.Model {
  headers: { [key: string]: string };
  /**
   * @remarks
   * HTTP Status Code
   */
  statusCode: number;
  event: $dara.SSEEvent;
  static names(): { [key: string]: string } {
    return {
      headers: 'headers',
      statusCode: 'statusCode',
      event: 'event',
    };
  }

  static types(): { [key: string]: any } {
    return {
      headers: { 'type': 'map', 'keyType': 'string', 'valueType': 'string' },
      statusCode: 'number',
      event: $dara.SSEEvent,
    };
  }

  validate() {
    if(this.headers) {
      $dara.Model.validateMap(this.headers);
    }
    $dara.Model.validateRequired("headers", this.headers);
    $dara.Model.validateRequired("statusCode", this.statusCode);
    $dara.Model.validateRequired("event", this.event);
    super.validate();
  }

  constructor(map?: { [key: string]: any }) {
    super(map);
  }
}

