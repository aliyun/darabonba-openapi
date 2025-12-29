<?php

namespace Darabonba\OpenApi\Tests;

use Darabonba\OpenApi\Utils;
use Darabonba\OpenApi\Sm3;
use AlibabaCloud\Dara\Request;
use AlibabaCloud\Dara\Util\BytesUtil;
use PHPUnit\Framework\TestCase;

/**
 * @internal
 * @coversNothing
 */
class UtilsTest extends TestCase
{
    /**
     * Test getEndpoint function
     */
    public function testGetEndpoint()
    {
        // Test internal endpoint
        $endpoint = Utils::getEndpoint('ecs.aliyuncs.com', false, 'internal');
        $this->assertEquals('ecs-internal.aliyuncs.com', $endpoint);

        // Test accelerate endpoint
        $endpoint = Utils::getEndpoint('oss.aliyuncs.com', true, 'accelerate');
        $this->assertEquals('oss-accelerate.aliyuncs.com', $endpoint);

        // Test normal endpoint
        $endpoint = Utils::getEndpoint('ecs.aliyuncs.com', false, 'public');
        $this->assertEquals('ecs.aliyuncs.com', $endpoint);

        // Test accelerate without flag
        $endpoint = Utils::getEndpoint('oss.aliyuncs.com', false, 'accelerate');
        $this->assertEquals('oss.aliyuncs.com', $endpoint);
    }

    /**
     * Test getThrottlingTimeLeft function
     */
    public function testGetThrottlingTimeLeft()
    {
        // Test with both headers
        $headers = array(
            'x-ratelimit-user-api' => 'Remaining:10,TimeLeft:5',
            'x-ratelimit-user' => 'Remaining:20,TimeLeft:3'
        );
        $timeLeft = Utils::getThrottlingTimeLeft($headers);
        $this->assertEquals(5, $timeLeft);

        // Test with empty headers
        $headers = array();
        $timeLeft = Utils::getThrottlingTimeLeft($headers);
        $this->assertEquals(0, $timeLeft);

        // Test with only one header
        $headers = array('x-ratelimit-user' => 'TimeLeft:10');
        $timeLeft = Utils::getThrottlingTimeLeft($headers);
        $this->assertEquals(10, $timeLeft);

        // Test with invalid value
        $headers = array('x-ratelimit-user-api' => 'Invalid');
        $timeLeft = Utils::getThrottlingTimeLeft($headers);
        $this->assertEquals(0, $timeLeft);
    }

    /**
     * Test hash function
     */
    public function testHash()
    {
        // Test SHA256
        $data = 'test message';
        $bytes = BytesUtil::from($data);
        $result = Utils::hash($bytes, 'ACS3-HMAC-SHA256');
        
        $this->checkInternalType('string', $result);
        $this->assertEquals(32, strlen($result)); // SHA256 produces 32 bytes

        // Test SM3
        $result = Utils::hash($bytes, 'ACS3-HMAC-SM3');
        $this->checkInternalType('string', $result);
        $this->assertEquals(32, strlen($result)); // SM3 produces 32 bytes

        // Test default case
        $result = Utils::hash($bytes, 'UNKNOWN');
        $this->checkInternalType('array', $result);
        $this->assertEmpty($result);
    }

    /**
     * Test getNonce function
     */
    public function testGetNonce()
    {
        $nonce1 = Utils::getNonce();
        $nonce2 = Utils::getNonce();

        $this->checkInternalType('string', $nonce1);
        $this->checkInternalType('string', $nonce2);
        $this->assertEquals(32, strlen($nonce1));
        $this->assertEquals(32, strlen($nonce2));
        $this->assertNotEquals($nonce1, $nonce2); // Should be unique
    }

    /**
     * Test getTimestamp function
     */
    public function testGetTimestamp()
    {
        $timestamp = Utils::getTimestamp();
        
        $this->checkInternalType('string', $timestamp);
        // Format: 2023-12-26T10:30:00Z
        $this->checkRegExp('/^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}Z$/', $timestamp);
    }

    /**
     * Test getDateUTCString function
     */
    public function testGetDateUTCString()
    {
        $dateString = Utils::getDateUTCString();
        
        $this->checkInternalType('string', $dateString);
        // Format: Mon, 26 Dec 2023 10:30:00 GMT
        $this->checkRegExp('/^\w{3}, \d{2} \w{3} \d{4} \d{2}:\d{2}:\d{2} \w+$/', $dateString);
    }

    /**
     * Test getROASignature function
     */
    public function testGetROASignature()
    {
        $stringToSign = "GET\n\n\n\nTue, 26 Dec 2023 10:30:00 GMT\n/test";
        $secret = "test-secret";
        
        $signature = Utils::getROASignature($stringToSign, $secret);
        
        $this->checkInternalType('string', $signature);
        $this->assertNotEmpty($signature);
    }

    /**
     * Test getRPCSignature function
     */
    public function testGetRPCSignature()
    {
        $signedParams = array(
            'Action' => 'DescribeInstances',
            'Version' => '2014-05-26',
            'Timestamp' => '2023-12-26T10:30:00Z'
        );
        $method = 'GET';
        $secret = 'test-secret';
        
        $signature = Utils::getRPCSignature($signedParams, $method, $secret);
        
        $this->checkInternalType('string', $signature);
        $this->assertNotEmpty($signature);
    }

    /**
     * Test toForm function
     */
    public function testToForm()
    {
        $filter = array(
            'key1' => 'value1',
            'key2' => 'value2',
            'nested' => array('key3' => 'value3')
        );
        
        $result = Utils::toForm($filter);
        
        $this->checkInternalType('string', $result);
        $this->checkStringContains('key1=value1', $result);
        $this->checkStringContains('key2=value2', $result);

        // Test with null
        $result = Utils::toForm(null);
        $this->assertEquals('', $result);
    }

    /**
     * Test query function
     */
    public function testQuery()
    {
        $filter = array(
            'key1' => 'value1',
            'key2' => 'value2',
            '_private' => 'ignore',
            'nested' => array('key3' => 'value3')
        );
        
        $result = Utils::query($filter);
        
        $this->checkInternalType('array', $result);
        $this->assertArrayHasKey('key1', $result);
        $this->assertArrayHasKey('key2', $result);
        $this->assertArrayNotHasKey('_private', $result);

        // Test with null
        $result = Utils::query(null);
        $this->checkInternalType('array', $result);
        $this->assertEmpty($result);
    }

    /**
     * Test arrayToStringWithSpecifiedStyle function
     */
    public function testArrayToStringWithSpecifiedStyle()
    {
        $array = array('a', 'b', 'c');
        
        // Test simple style (comma-separated)
        $result = Utils::arrayToStringWithSpecifiedStyle($array, 'prefix', 'simple');
        $this->assertEquals('a,b,c', $result);

        // Test spaceDelimited style
        $result = Utils::arrayToStringWithSpecifiedStyle($array, 'prefix', 'spaceDelimited');
        $this->assertEquals('a b c', $result);

        // Test pipeDelimited style
        $result = Utils::arrayToStringWithSpecifiedStyle($array, 'prefix', 'pipeDelimited');
        $this->assertEquals('a|b|c', $result);

        // Test json style
        $result = Utils::arrayToStringWithSpecifiedStyle($array, 'prefix', 'json');
        $this->checkStringContains('a', $result);
        $this->checkStringContains('b', $result);

        // Test with null
        $result = Utils::arrayToStringWithSpecifiedStyle(null, 'prefix', 'simple');
        $this->assertEquals('', $result);
    }

    /**
     * Test parseToMap function
     */
    public function testParseToMap()
    {
        $input = array(
            'key1' => 'value1',
            'key2' => array('nested' => 'value2')
        );
        
        $result = Utils::parseToMap($input);
        
        $this->checkInternalType('array', $result);
        $this->assertEquals('value1', $result['key1']);
        $this->checkInternalType('array', $result['key2']);

        // Test with null
        $result = Utils::parseToMap(null);
        $this->checkInternalType('array', $result);
        // parseToMap(null) returns array() from parse function
        $this->assertTrue(is_array($result));
    }

    /**
     * Test getUserAgent function
     */
    public function testGetUserAgent()
    {
        // Test with custom user agent
        $userAgent = Utils::getUserAgent('MyApp/1.0');
        
        $this->checkInternalType('string', $userAgent);
        $this->checkStringContains('AlibabaCloud', $userAgent);
        $this->checkStringContains('PHP/', $userAgent);
        $this->checkStringContains('MyApp/1.0', $userAgent);

        // Test with empty user agent
        $userAgent = Utils::getUserAgent('');
        $this->checkStringContains('AlibabaCloud', $userAgent);
        $this->assertFalse(strpos($userAgent, 'MyApp') !== false);
    }

    /**
     * Test toArray function
     */
    public function testToArray()
    {
        $input = array(
            'key1' => 'value1',
            'key2' => array('nested' => 'value2')
        );
        
        $result = Utils::toArray($input);
        
        $this->checkInternalType('array', $result);
        $this->assertEquals($input, $result);

        // Test with non-array
        $result = Utils::toArray('string');
        $this->assertEquals('string', $result);
    }

    /**
     * Test stringifyMapValue function
     */
    public function testStringifyMapValue()
    {
        $map = array(
            'int' => 123,
            'bool' => true,
            'null' => null,
            'string' => 'test'
        );
        
        $result = Utils::stringifyMapValue($map);
        
        $this->checkInternalType('array', $result);
        $this->assertEquals('123', $result['int']);
        $this->assertEquals('true', $result['bool']);
        $this->assertEquals('', $result['null']);
        $this->assertEquals('test', $result['string']);

        // Test with null
        $result = Utils::stringifyMapValue(null);
        $this->checkInternalType('array', $result);
        $this->assertEmpty($result);
    }

    /**
     * Test getEndpointRules function
     */
    public function testGetEndpointRules()
    {
        // Test regional endpoint
        $result = Utils::getEndpointRules('ecs', 'cn-hangzhou', 'regional', '');
        $this->assertEquals('ecs.cn-hangzhou.aliyuncs.com', $result);

        // Test non-regional endpoint
        $result = Utils::getEndpointRules('oss', '', 'public', '');
        $this->assertEquals('oss.aliyuncs.com', $result);

        // Test with vpc network
        $result = Utils::getEndpointRules('ecs', 'cn-beijing', 'regional', 'vpc');
        $this->assertEquals('ecs-vpc.cn-beijing.aliyuncs.com', $result);

        // Test with internal network
        $result = Utils::getEndpointRules('rds', 'cn-shanghai', 'regional', 'internal');
        $this->assertEquals('rds-internal.cn-shanghai.aliyuncs.com', $result);

        // Test with public network (should be ignored)
        $result = Utils::getEndpointRules('ecs', 'cn-hangzhou', 'regional', 'public');
        $this->assertEquals('ecs.cn-hangzhou.aliyuncs.com', $result);
    }

    /**
     * Test getEndpointRules with missing regionId
     */
    public function testGetEndpointRulesThrowsException()
    {
        try {
            Utils::getEndpointRules('ecs', '', 'regional', '');
            $this->fail('Expected RuntimeException was not thrown');
        } catch (\RuntimeException $e) {
            $this->checkStringContains('RegionId is empty', $e->getMessage());
        }
    }

    /**
     * Test getProductEndpoint function
     */
    public function testGetProductEndpoint()
    {
        // Test with provided endpoint
        $result = Utils::getProductEndpoint('ecs', 'cn-hangzhou', 'regional', '', null, array(), 'custom.endpoint.com');
        $this->assertEquals('custom.endpoint.com', $result);

        // Test with endpoint map
        $endpointMap = array('cn-hangzhou' => 'map.endpoint.com');
        $result = Utils::getProductEndpoint('ecs', 'cn-hangzhou', 'regional', '', null, $endpointMap, null);
        $this->assertEquals('map.endpoint.com', $result);

        // Test fallback to getEndpointRules
        $result = Utils::getProductEndpoint('ecs', 'cn-hangzhou', 'regional', '', null, array(), null);
        $this->assertEquals('ecs.cn-hangzhou.aliyuncs.com', $result);
    }

    /**
     * Test sign function
     */
    public function testSign()
    {
        $secret = 'test-secret';
        $str = 'test-string';

        // Test HMAC-SHA256
        $result = Utils::sign($secret, $str, 'ACS3-HMAC-SHA256');
        $this->checkInternalType('string', $result);
        $this->assertEquals(32, strlen($result));

        // Test HMAC-SM3
        $result = Utils::sign($secret, $str, 'ACS3-HMAC-SM3');
        $this->checkInternalType('string', $result);
    }

    /**
     * Test getStringToSign function
     */
    public function testGetStringToSign()
    {
        $request = new Request();
        $request->method = 'GET';
        $request->pathname = '/test';
        $request->query = array('key' => 'value');
        $request->headers = array(
            'accept' => 'application/json',
            'content-type' => 'application/json',
            'date' => 'Tue, 26 Dec 2023 10:30:00 GMT',
            'x-acs-custom' => 'custom-value'
        );

        $result = Utils::getStringToSign($request);
        
        $this->checkInternalType('string', $result);
        $this->checkStringContains('GET', $result);
        $this->checkStringContains('/test', $result);
        $this->checkStringContains('x-acs-custom', $result);
    }

    /**
     * Test getAuthorization function
     */
    public function testGetAuthorization()
    {
        $request = new Request();
        $request->method = 'GET';
        $request->pathname = '/test';
        $request->query = array('key' => 'value');
        $request->headers = array(
            'host' => 'test.aliyuncs.com',
            'content-type' => 'application/json',
            'x-acs-custom' => 'custom-value'
        );

        $result = Utils::getAuthorization(
            $request,
            'ACS3-HMAC-SHA256',
            'payload-hash',
            'test-ak',
            'test-sk'
        );
        
        $this->checkInternalType('string', $result);
        $this->checkStringContains('ACS3-HMAC-SHA256', $result);
        $this->checkStringContains('Credential=test-ak', $result);
        $this->checkStringContains('SignedHeaders=', $result);
        $this->checkStringContains('Signature=', $result);
    }

    /**
     * Test Sm3 class
     */
    public function testSm3()
    {
        $sm3 = new Sm3();
        
        // Test known value
        $result = $sm3->sign('abc');
        $this->assertEquals('66c7f0f462eeedd9d1f2d46bdc10e4e24167c4875cf2f7a2297da02b8f4ba8e0', $result);

        // Test another value
        $result = $sm3->sign('test');
        $this->checkInternalType('string', $result);
        $this->assertEquals(64, strlen($result)); // Hex string of 32 bytes
    }

    /**
     * Helper method for string contains assertion (cross-version compatibility)
     */
    private function checkStringContains($needle, $haystack, $message = '')
    {
        if (method_exists($this, 'assertStringContainsString')) {
            $this->assertStringContainsString($needle, $haystack, $message);
        } else {
            // Fallback for older PHPUnit
            $this->assertContains($needle, $haystack, $message);
        }
    }

    /**
     * Helper method for PHP 5.5 compatibility
     * assertInternalType was deprecated in PHPUnit 8
     */
    private function checkInternalType($type, $value, $message = '')
    {
        if (method_exists($this, 'assertIsString') && $type === 'string') {
            $this->assertIsString($value, $message);
        } elseif (method_exists($this, 'assertIsArray') && $type === 'array') {
            $this->assertIsArray($value, $message);
        } elseif (method_exists($this, 'assertIsInt') && $type === 'int') {
            $this->assertIsInt($value, $message);
        } else {
            // Fallback for older PHPUnit - use parent method or assertTrue
            if (method_exists('PHPUnit_Framework_Assert', 'assertInternalType')) {
                parent::assertInternalType($type, $value, $message);
            } else {
                $typeMap = array(
                    'string' => 'is_string',
                    'array' => 'is_array',
                    'int' => 'is_int',
                    'integer' => 'is_int',
                    'bool' => 'is_bool',
                    'boolean' => 'is_bool'
                );
                if (isset($typeMap[$type])) {
                    $this->assertTrue(call_user_func($typeMap[$type], $value), $message);
                }
            }
        }
    }

    /**
     * Helper method for PHP 5.5 compatibility
     * assertRegExp was deprecated in PHPUnit 9
     */
    private function checkRegExp($pattern, $string, $message = '')
    {
        if (method_exists($this, 'assertMatchesRegularExpression')) {
            $this->assertMatchesRegularExpression($pattern, $string, $message);
        } else {
            // Fallback for older PHPUnit - use assertTrue with preg_match
            $this->assertTrue((bool) preg_match($pattern, $string), $message);
        }
    }
}