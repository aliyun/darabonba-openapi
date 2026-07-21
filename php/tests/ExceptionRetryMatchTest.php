<?php

namespace Darabonba\OpenApi\Tests;

use AlibabaCloud\Dara\Dara;
use AlibabaCloud\Dara\RetryPolicy\RetryCondition;
use AlibabaCloud\Dara\RetryPolicy\RetryOptions;
use AlibabaCloud\Dara\RetryPolicy\RetryPolicyContext;
use Darabonba\OpenApi\Exceptions\AlibabaCloudException;
use Darabonba\OpenApi\Exceptions\ClientException;
use Darabonba\OpenApi\Exceptions\ServerException;
use Darabonba\OpenApi\Exceptions\ThrottlingException;
use PHPUnit\Framework\TestCase;

/**
 * Regression: OpenAPI exceptions must expose errCode/name so Dara::shouldRetry
 * can match retryCondition.errorCode after the first attempt (retryCount > 0).
 *
 * @internal
 * @covers \Darabonba\OpenApi\Exceptions\AlibabaCloudException
 * @covers \Darabonba\OpenApi\Exceptions\ThrottlingException
 * @covers \Darabonba\OpenApi\Exceptions\ClientException
 * @covers \Darabonba\OpenApi\Exceptions\ServerException
 */
class ExceptionRetryMatchTest extends TestCase
{
    private static function baseMap(array $extra = [])
    {
        return array_merge([
            'statusCode' => 400,
            'code' => 'Throttling',
            'message' => 'code: 400, Throttling request id: rid',
            'description' => '',
            'requestId' => 'rid',
            'data' => ['Code' => 'Throttling'],
        ], $extra);
    }

    private static function throttlingRetryOptions()
    {
        return new RetryOptions([
            'retryable' => true,
            'maxAttempts' => 3,
            'retryCondition' => [new RetryCondition([
                'maxAttempts' => 3,
                'errorCode' => ['Throttling', 'Throttling.User', 'Throttling.Api'],
                'maxDelay' => 60000,
            ])],
        ]);
    }

    public function testAlibabaCloudExceptionSyncsCodeToErrCodeAndDefaultName()
    {
        $ex = new AlibabaCloudException(self::baseMap([
            'detail' => 'throttled',
            'description' => 'slow down',
        ]));
        $this->assertEquals('Throttling', $ex->code);
        $this->assertEquals('Throttling', $ex->getErrCode());
        $this->assertEquals('AlibabaCloudException', $ex->getName());
        $this->assertEquals(400, $ex->getStatusCode());
        $this->assertEquals('throttled', $ex->getDetail());
        $this->assertEquals('slow down', $ex->getDescription());
        $this->assertEquals('rid', $ex->getRequestId());
    }

    public function testAlibabaCloudExceptionPreservesExplicitErrCodeAndName()
    {
        $ex = new AlibabaCloudException(self::baseMap([
            'code' => 'Throttling',
            'errCode' => 'CustomErr',
            'name' => 'CustomName',
        ]));
        $this->assertEquals('Throttling', $ex->code);
        $this->assertEquals('CustomErr', $ex->getErrCode());
        $this->assertEquals('CustomName', $ex->getName());
    }

    public function testThrottlingExceptionErrCodeMatchesShouldRetryAfterFirstAttempt()
    {
        $ex = new ThrottlingException(self::baseMap([
            'retryAfter' => 1000,
        ]));
        $this->assertEquals('Throttling', $ex->getErrCode());
        $this->assertEquals('ThrottlingException', $ex->getName());
        $this->assertEquals(1000, $ex->getRetryAfter());

        $options = self::throttlingRetryOptions();
        // First attempt always allowed.
        $ctx0 = new RetryPolicyContext([
            'retriesAttempted' => 0,
            'exception' => $ex,
        ]);
        $this->assertTrue(Dara::shouldRetry($options, $ctx0));

        // Second attempt must match errorCode via getErrCode() (was broken when only `code` was set).
        $ctx1 = new RetryPolicyContext([
            'retriesAttempted' => 1,
            'exception' => $ex,
        ]);
        $this->assertTrue(Dara::shouldRetry($options, $ctx1));
        $this->assertEquals(1000, Dara::getBackoffDelay($options, $ctx1));
    }

    public function testThrottlingUserApiErrorCodesAlsoMatch()
    {
        $options = self::throttlingRetryOptions();
        foreach (['Throttling.User', 'Throttling.Api'] as $code) {
            $ex = new ThrottlingException(self::baseMap([
                'code' => $code,
                'retryAfter' => 500,
            ]));
            $ctx = new RetryPolicyContext([
                'retriesAttempted' => 1,
                'exception' => $ex,
            ]);
            $this->assertTrue(Dara::shouldRetry($options, $ctx), "should retry for code=$code");
            $this->assertEquals(500, Dara::getBackoffDelay($options, $ctx));
        }
    }

    public function testUnmatchedErrorCodeDoesNotRetryAfterFirstAttempt()
    {
        $ex = new ThrottlingException(self::baseMap([
            'code' => 'InvalidParameter',
            'retryAfter' => 1000,
        ]));
        $options = self::throttlingRetryOptions();
        $ctx = new RetryPolicyContext([
            'retriesAttempted' => 1,
            'exception' => $ex,
        ]);
        $this->assertFalse(Dara::shouldRetry($options, $ctx));
    }

    public function testClientAndServerExceptionSetLogicalNames()
    {
        $client = new ClientException(self::baseMap([
            'code' => 'Forbidden.RAM',
            'accessDeniedDetail' => ['foo' => 'bar'],
        ]));
        $this->assertEquals('ClientException', $client->getName());
        $this->assertEquals('Forbidden.RAM', $client->getErrCode());
        $this->assertEquals(['foo' => 'bar'], $client->getAccessDeniedDetail());

        $server = new ServerException(self::baseMap([
            'statusCode' => 500,
            'code' => 'InternalError',
        ]));
        $this->assertEquals('ServerException', $server->getName());
        $this->assertEquals('InternalError', $server->getErrCode());
    }
}
