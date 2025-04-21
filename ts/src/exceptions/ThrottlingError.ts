// This file is auto-generated, don't edit it
import * as $dara from '@darabonba/typescript';
import { AlibabaCloudError } from "./AlibabaCloudError";


export class ThrottlingError extends AlibabaCloudError {
  retryAfter?: number;

  constructor(map?: { [key: string]: any }) {
    super(map);
    this.name = "ThrottlingError";
    Object.setPrototypeOf(this, ThrottlingError.prototype);
    this.retryAfter = map.retryAfter;
  }
}

