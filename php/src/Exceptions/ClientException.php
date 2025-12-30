<?php

// This file is auto-generated, don't edit it. Thanks.
 
namespace Darabonba\OpenApi\Exceptions;
class ClientException extends AlibabaCloudException {
  /**
  * @var mixed[]
  */
  public $accessDeniedDetail;

  public function __construct($map)
  {
    parent::__construct($map);
    $this->accessDeniedDetail = $map['accessDeniedDetail'];
  }

  /**
  * @return mixed[]
  */
  public function getAccessDeniedDetail()
  {
    return $this->accessDeniedDetail;
  }
}
