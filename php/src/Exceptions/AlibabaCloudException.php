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
    if (isset($map['code']) && !isset($map['errCode'])) {
      $map['errCode'] = $map['code'];
    }
    parent::__construct($map);
    $this->statusCode = isset($map['statusCode']) ? $map['statusCode'] : 0;
    $this->code = isset($map['code']) ? $map['code'] : '';
    $this->message = isset($map['message']) ? $map['message'] : '';
    $this->detail = isset($map['detail']) ? $map['detail'] : null;
    $this->description = isset($map['description']) ? $map['description'] : null;
    $this->requestId = isset($map['requestId']) ? $map['requestId'] : null;
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
