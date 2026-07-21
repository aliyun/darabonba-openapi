<?php

// This file is auto-generated, don't edit it. Thanks.

namespace Darabonba\OpenApi\Exceptions;

use AlibabaCloud\Dara\Exception\DaraException;

class AlibabaCloudException extends DaraException
{
  /**
   * @var int
   */
  public $statusCode;
  /**
   * @var string
   */
  public $code;
  /**
   * @var string
   */
  public $message;
  /**
   * @var string
   */
  public $detail;
  /**
   * @var string
   */
  public $description;
  /**
   * @var string
   */
  protected $requestId;

  public function __construct($map)
  {
    // Dara::shouldRetry / getBackoffDelay match on getErrCode()/getName(),
    // while OpenAPI exceptions store the business error code in `code`.
    // Sync code → errCode so throttling (and other) retry conditions can hit.
    if (isset($map['code']) && !isset($map['errCode'])) {
      $map['errCode'] = $map['code'];
    }
    if (!isset($map['name'])) {
      $map['name'] = 'AlibabaCloudException';
    }
    parent::__construct($map);
    $this->statusCode = $map['statusCode'];
    $this->code = $map['code'];
    $this->message = $map['message'];
    $this->detail = isset($map['detail']) ? $map['detail'] : null;
    $this->description = $map['description'];
    $this->requestId = $map['requestId'];
  }

  /**
   * @return int
   */
  public function getStatusCode()
  {
    return $this->statusCode;
  }
  /**
   * @return string
   */
  public function getDetail()
  {
    return $this->detail;
  }
  /**
   * @return string
   */
  public function getDescription()
  {
    return $this->description;
  }
  /**
   * @return string
   */
  public function getRequestId()
  {
    return $this->requestId;
  }
}
