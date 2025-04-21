// This file is auto-generated, don't edit it
import * as $dara from '@darabonba/typescript';


export class AlibabaCloudError extends $dara.ResponseError {
  statusCode?: number;
  code: string;
  message: string;
  description?: string;
  requestId?: string;

  constructor(map?: { [key: string]: any }) {
    super(map);
    this.name = "AlibabaCloudError";
    Object.setPrototypeOf(this, AlibabaCloudError.prototype);
    this.statusCode = map.statusCode;
    this.code = map.code;
    this.description = map.description;
    this.requestId = map.requestId;
  }
}

