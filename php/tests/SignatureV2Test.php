<?php

use AlibabaCloud\Tea\Utils\Utils\ExtendsParameters;
use AlibabaCloud\Tea\Utils\Utils\RuntimeOptions;
use Darabonba\OpenApi\Models\Config;
use Darabonba\OpenApi\Models\GlobalParameters;
use Darabonba\OpenApi\Models\OpenApiRequest;
use Darabonba\OpenApi\Models\Params;
use Darabonba\OpenApi\OpenApiClient;
use PHPUnit\Framework\TestCase;

class SignatureV2UtilProbe
{
    public static function getRPCSignatureWithSHA256($params, $method, $secret) { return 'util-rpc'; }
    public static function getROASignatureWithSHA256($stringToSign, $secret) { return 'util-roa'; }
}

class SignatureV2Test extends TestCase
{
    private static $process;
    private static $pipes;
    private static $port;

    /** @beforeClass */
    public static function startServer()
    {
        self::$process = proc_open(escapeshellarg(PHP_BINARY) . ' ' . escapeshellarg(__DIR__ . '/Mock/SignatureV2Server.php'), [['pipe', 'r'], ['pipe', 'w'], ['pipe', 'w']], self::$pipes);
        self::$port = (int)trim(fgets(self::$pipes[1]));
    }

    /** @afterClass */
    public static function stopServer()
    {
        proc_terminate(self::$process);
        foreach (self::$pipes as $pipe) fclose($pipe);
        proc_close(self::$process);
    }

    public function testV2AlgorithmsAndHeaderPrecedence()
    {
        $config = new Config(['accessKeyId' => 'ak', 'accessKeySecret' => 'secret', 'securityToken' => 'token', 'protocol' => 'HTTP', 'endpoint' => 'signatures.example.invalid', 'signatureAlgorithm' => 'v2', 'globalParameters' => new GlobalParameters(['headers' => []])]);
        $runtime = new RuntimeOptions(['httpProxy' => 'http://127.0.0.1:' . self::$port, 'extendsParameters' => new ExtendsParameters(['headers' => []])]);
        $request = new OpenApiRequest(['query' => ['special' => '中文/%!~ +@', 'empty' => ''], 'body' => ['body' => 'test'], 'headers' => []]);
        $client = new OpenApiClient($config);
        foreach (['HMAC-SHA256', 'HMAC-SHA1'] as $method) {
            if ($method === 'HMAC-SHA1') {
                $config->globalParameters->headers['x-acs-signature-method'] = 'HMAC-SHA256';
                $runtime->extendsParameters->headers['x-acs-signature-method'] = 'HMAC-SHA256';
                $request->headers['X-Acs-Signature-Method'] = $method;
            }
            foreach (['RPC', 'ROA'] as $style) foreach (['json', 'formData'] as $bodyType) {
                $params = new Params(['action' => 'TestAPI', 'version' => '2022-06-01', 'protocol' => 'HTTP', 'pathname' => '/test', 'method' => 'POST', 'authType' => 'AK', 'style' => $style, 'reqBodyType' => $bodyType, 'bodyType' => 'json']);
                $this->assertSignature($client->callApi($params, $request, $runtime)['body'], $style, $method);
            }
        }
        $request->headers['x-acs-signature-method'] = 'HMAC-SHA1';
        $this->assertSignature($client->callApi($params, $request, $runtime)['body'], 'ROA', 'HMAC-SHA1');
        $request->headers['x-acs-signature-method'] = 'HMAC-SHA256';
        try { $client->callApi($params, $request, $runtime); $this->fail('conflicting methods accepted'); }
        catch (\Exception $error) { $this->assertTrue(strpos($error->getMessage(), 'Conflicting signature V2 algorithms') !== false); }
        unset($request->headers['x-acs-signature-method']);
        $request->headers['X-Acs-Signature-Method'] = 'unsupported';
        try { $client->callApi($params, $request, $runtime); $this->fail('unsupported method accepted'); }
        catch (\Exception $error) { $this->assertTrue(strpos($error->getMessage(), 'Unsupported signature V2 algorithm') !== false); }
    }

    public function testNonHmacHeadersRemainUnchanged()
    {
        $config = new Config(['bearerToken' => 'token', 'protocol' => 'HTTP', 'endpoint' => 'signatures.example.invalid', 'signatureAlgorithm' => 'v2']);
        $request = new OpenApiRequest(['headers' => ['x-acs-signature-method' => 'custom']]);
        $params = new Params(['action' => 'TestAPI', 'version' => '2022-06-01', 'protocol' => 'HTTP', 'pathname' => '/test', 'method' => 'POST', 'authType' => 'AK', 'style' => 'ROA', 'reqBodyType' => 'json', 'bodyType' => 'json']);
        $client = new OpenApiClient($config);
        $runtime = new RuntimeOptions(['httpProxy' => 'http://127.0.0.1:' . self::$port]);
        $this->assertEquals('custom', $client->callApi($params, $request, $runtime)['body']['headers']['x-acs-signature-method']);
        $params->authType = 'Anonymous';
        $this->assertEquals('custom', $client->callApi($params, $request, $runtime)['body']['headers']['x-acs-signature-method']);
    }

    public function testRpcQuerySignatureMethod()
    {
        $config = new Config(['accessKeyId' => 'ak', 'accessKeySecret' => 'secret', 'securityToken' => 'token', 'protocol' => 'HTTP', 'endpoint' => 'signatures.example.invalid', 'signatureAlgorithm' => 'v2', 'globalParameters' => new GlobalParameters(['queries' => []])]);
        $runtime = new RuntimeOptions(['httpProxy' => 'http://127.0.0.1:' . self::$port, 'extendsParameters' => new ExtendsParameters(['queries' => []])]);
        $request = new OpenApiRequest(['query' => ['test' => 'value'], 'body' => ['body' => 'test'], 'headers' => []]);
        $client = new OpenApiClient($config);
        $params = new Params(['action' => 'TestAPI', 'version' => '2022-06-01', 'protocol' => 'HTTP', 'pathname' => '/', 'method' => 'POST', 'authType' => 'AK', 'style' => 'RPC', 'reqBodyType' => 'formData', 'bodyType' => 'json']);
        $config->globalParameters->queries['SignatureMethod'] = 'HMAC-SHA1';
        $this->assertSignature($client->callApi($params, $request, $runtime)['body'], 'RPC', 'HMAC-SHA1');
        unset($config->globalParameters->queries['SignatureMethod']);
        $runtime->extendsParameters->queries['SignatureMethod'] = 'HMAC-SHA1';
        $this->assertSignature($client->callApi($params, $request, $runtime)['body'], 'RPC', 'HMAC-SHA1');
        unset($runtime->extendsParameters->queries['SignatureMethod']);
        $request->query['SignatureMethod'] = 'HMAC-SHA1';
        $this->assertSignature($client->callApi($params, $request, $runtime)['body'], 'RPC', 'HMAC-SHA1');
        $config->globalParameters->queries['SignatureMethod'] = 'HMAC-SHA1';
        $request->query['SignatureMethod'] = 'legacy';
        $this->assertSignature($client->callApi($params, $request, $runtime)['body'], 'RPC', 'HMAC-SHA256');
        $request->query['SignatureMethod'] = '';
        $this->assertSignature($client->callApi($params, $request, $runtime)['body'], 'RPC', 'HMAC-SHA256');
        $request->query['SignatureMethod'] = 'HMAC-SHA1';
        $request->headers['X-Acs-Signature-Method'] = 'HMAC-SHA256';
        $this->assertSignature($client->callApi($params, $request, $runtime)['body'], 'RPC', 'HMAC-SHA256');
        $request->headers['X-Acs-Signature-Method'] = '';
        try { $client->callApi($params, $request, $runtime); $this->fail('empty signature method accepted'); }
        catch (\Exception $error) { $this->assertTrue(strpos($error->getMessage(), 'Unsupported signature V2 algorithm') !== false); }
    }

    public function testV3RequestRemainsAvailable()
    {
        $config = new Config(['accessKeyId' => 'ak', 'accessKeySecret' => 'secret', 'protocol' => 'HTTP', 'endpoint' => 'signatures.example.invalid']);
        $request = new OpenApiRequest(['query' => ['q' => 'test'], 'body' => ['body' => 'test']]);
        $params = new Params(['action' => 'TestAPI', 'version' => '2022-06-01', 'protocol' => 'HTTP', 'pathname' => '/test', 'method' => 'POST', 'authType' => 'AK', 'style' => 'RPC', 'reqBodyType' => 'json', 'bodyType' => 'json']);
        $runtime = new RuntimeOptions(['httpProxy' => 'http://127.0.0.1:' . self::$port]);
        $captured = (new OpenApiClient($config))->callApi($params, $request, $runtime)['body'];
        $this->assertTrue(strpos($captured['headers']['authorization'], 'ACS3-HMAC-SHA256 ') === 0);
    }

    /**
     * @runInSeparateProcess
     * @preserveGlobalState disabled
     */
    public function testSha256PrefersAvailableUtilMethods()
    {
        class_alias(SignatureV2UtilProbe::class, 'AlibabaCloud\\OpenApiUtil\\OpenApiUtilClient');
        $rpc = new \ReflectionMethod(OpenApiClient::class, 'rpcSignatureWithSHA256');
        $roa = new \ReflectionMethod(OpenApiClient::class, 'roaSignatureWithSHA256');
        $rpc->setAccessible(true);
        $roa->setAccessible(true);
        $this->assertSame('util-rpc', $rpc->invoke(null, [], 'GET', 'secret'));
        $this->assertSame('util-roa', $roa->invoke(null, 'payload', 'secret'));
    }

    private function assertSignature($captured, $style, $method)
    {
        $parts = parse_url($captured['path']);
        parse_str(isset($parts['query']) ? $parts['query'] : '', $query);
        $digest = $method === 'HMAC-SHA1' ? 'sha1' : 'sha256';
        if ($style === 'RPC') {
            $this->assertEquals($method, $query['SignatureMethod']);
            $signature = $query['Signature'];
            unset($query['Signature']);
            parse_str($captured['body'], $body);
            $query = array_merge($query, $body);
            ksort($query);
            $canonical = [];
            foreach ($query as $key => $value) $canonical[] = rawurlencode($key) . '=' . rawurlencode($value);
            $stringToSign = $captured['method'] . '&%2F&' . rawurlencode(implode('&', $canonical));
            $this->assertEquals(base64_encode(hash_hmac($digest, $stringToSign, 'secret&', true)), $signature);
        } else {
            $headers = $captured['headers'];
            $this->assertEquals($method, $headers['x-acs-signature-method']);
            $canonicalHeaders = [];
            foreach ($headers as $key => $value) if (strpos($key, 'x-acs-') === 0) $canonicalHeaders[$key] = $key . ':' . trim($value) . "\n";
            ksort($canonicalHeaders);
            ksort($query);
            $canonicalQuery = [];
            foreach ($query as $key => $value) $canonicalQuery[] = $value === '' ? $key : $key . '=' . $value;
            $resource = $parts['path'] . ($canonicalQuery ? '?' . implode('&', $canonicalQuery) : '');
            $stringToSign = implode("\n", [$captured['method'], $headers['accept'], isset($headers['content-md5']) ? $headers['content-md5'] : '', isset($headers['content-type']) ? $headers['content-type'] : '', $headers['date']]) . "\n" . implode('', $canonicalHeaders) . $resource;
            $this->assertEquals('acs ak:' . base64_encode(hash_hmac($digest, $stringToSign, 'secret', true)), $headers['authorization']);
        }
    }
}
