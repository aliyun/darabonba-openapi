// This file is auto-generated, don't edit it
import * as $dara from '@darabonba/typescript';
import { AlibabaCloudError } from "./AlibabaCloudError";


export class ClientError extends AlibabaCloudError {
  accessDeniedDetail?: { [key: string]: any };

  constructor(map?: { [key: string]: any }) {
    super(map);
    this.name = "ClientError";
    Object.setPrototypeOf(this, ClientError.prototype);
    this.accessDeniedDetail = map.accessDeniedDetail;
  }
}

