<?php

// This file is auto-generated, don't edit it. Thanks.
 
namespace Darabonba\OpenApi\Exceptions;
class ThrottlingException extends AlibabaCloudException {
  /**
  * @var int
  */
  protected $retryAfter;

  public function __construct($map)
  {
    parent::__construct($map);
    $this->retryAfter = $map['retryAfter'];
  }

  /**
  * @return int
  */
  public function getRetryAfter()
  {
    return $this->retryAfter;
  }
}
