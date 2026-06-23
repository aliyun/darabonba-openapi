<?php

// This file is auto-generated, don't edit it. Thanks.

namespace Darabonba\OpenApi\Exceptions;

class ThrottlingException extends AlibabaCloudException
{
  /**
   * @var int
   */
  protected $retryAfter;

  public function __construct($map)
  {
    $map['name'] = 'ThrottlingException';
    parent::__construct($map);
    $this->retryAfter = isset($map['retryAfter']) ? $map['retryAfter'] : null;
  }

  /**
   * @return int
   */
  public function getRetryAfter()
  {
    return $this->retryAfter;
  }
}
