<?php

namespace Darabonba\OpenApi\Tests;

use AlibabaCloud\Dara\Dara;
use AlibabaCloud\Dara\Exception\DaraUnableRetryException;
use AlibabaCloud\Dara\Models\FileField;
use AlibabaCloud\Dara\Models\RuntimeOptions;
use Darabonba\OpenApi\Exceptions\ClientException;
use Darabonba\OpenApi\Models\Config;
use Darabonba\OpenApi\OpenApiClient;
use GuzzleHttp\Promise\FulfilledPromise;
use GuzzleHttp\Psr7\Response;
use GuzzleHttp\Psr7\Utils as Psr7;
use PHPUnit\Framework\TestCase;

// 与产品 Client 相同，经子类访问 protected 方法，不替换上传实现。
class PrivateLinkProductClient extends OpenApiClient
{
    public function resolve($runtime)
    {
        return $this->getOSSPrivateUrl($runtime);
    }

    public function upload($url, $runtime = null)
    {
        return $this->postOSSObjectWithURL($url, 'test-bucket', array(
            'host' => 'oss-cn-hangzhou.aliyuncs.com', 'OSSAccessKeyId' => 'test-id',
            'policy' => 'test-policy', 'Signature' => 'test-signature',
            'key' => 'dir/file.bin', 'success_action_status' => '201',
            'file' => new FileField(array(
                'filename' => 'dir/file.bin', 'contentType' => 'application/octet-stream',
                'content' => Psr7::streamFor("\x00\x01\xff\x0a"),
            )),
        ), $runtime === null ? new RuntimeOptions() : $runtime);
    }
}

class OSSPrivateUrlTest extends TestCase
{
    private $calls = array();

    private function withTransport($status, $test)
    {
        $this->calls = array();
        // 只替换 Guzzle 传输边界，所有目标均不访问网络。
        Dara::config(array('handler' => function ($request, $options) use ($status) {
            $this->calls[] = array('request' => $request, 'body' => (string) $request->getBody(), 'options' => $options);
            $xml = $status === 201
                ? '<PostResponse><Key>dir/file.bin</Key></PostResponse>'
                : '<Error><Code>AccessDenied</Code><Message>denied</Message><RequestId>req-id</RequestId><HostId>host-id</HostId></Error>';
            return new FulfilledPromise(new Response($status, array('Content-Type' => 'application/xml'), $xml));
        }));
        try {
            $test();
        } finally {
            Dara::config(array());
        }
    }

    public function testRuntimePriorityAndMapping()
    {
        $client = new PrivateLinkProductClient(new Config(array('ossPrivateUrl' => 'http://config.example')));
        $this->assertSame('http://config.example', $client->resolve(new RuntimeOptions()));
        $this->assertSame('http://config.example', $client->resolve(new RuntimeOptions(array('ossPrivateUrl' => ''))));
        $this->assertSame('https://runtime.example', $client->resolve(new RuntimeOptions(array('ossPrivateUrl' => 'https://runtime.example'))));
        $this->assertNull((new PrivateLinkProductClient(new Config()))->resolve(new RuntimeOptions()));
        foreach (array('', 'https://pvl.example/proxy') as $url) {
            $this->assertSame($url, (new Config(array('ossPrivateUrl' => $url)))->toMap()['ossPrivateUrl']);
            $this->assertSame($url, (new RuntimeOptions(array('ossPrivateUrl' => $url)))->toMap()['ossPrivateUrl']);
        }
        $this->assertArrayNotHasKey('ossPrivateUrl', (new Config())->toMap());
    }

    public function testPathStyleAndBinaryMultipart()
    {
        $this->withTransport(201, function () {
            $client = new PrivateLinkProductClient(new Config());
            foreach (array('', '/', '/proxy', '/proxy/') as $prefix) {
                $result = $client->upload('http://pvl.example:8080' . $prefix);
                $this->assertSame(array('Key' => 'dir/file.bin'), $result);
                $record = $this->calls[count($this->calls) - 1];
                $this->assertSame('POST', $record['request']->getMethod());
                $this->assertSame('pvl.example:8080', $record['request']->getHeaderLine('Host'));
                $this->assertSame(rtrim($prefix, '/') . '/test-bucket/', $record['request']->getUri()->getPath());
                foreach (array('OSSAccessKeyId', 'policy', 'Signature', 'key', 'success_action_status', 'file') as $name) {
                    $this->assertNotFalse(strpos($record['body'], 'name="' . $name . '"'));
                }
                $this->assertNotFalse(strpos($record['body'], 'test-policy'));
                $this->assertNotFalse(strpos($record['body'], "\x00\x01\xff\x0a"));
            }
        });
    }

    public function testHttpsAndTransportOptions()
    {
        $this->withTransport(201, function () {
            $client = new PrivateLinkProductClient(new Config(array('readTimeout' => 4000, 'connectTimeout' => 2000)));
            foreach (array('https://pvl.example', 'https://pvl.example:8443/proxy') as $url) {
                $client->upload($url, new RuntimeOptions(array(
                    'readTimeout' => 7000, 'connectTimeout' => 3000, 'ignoreSSL' => true,
                    'httpProxy' => 'http://proxy.example:8888', 'httpsProxy' => 'http://proxy.example:8888',
                )));
                $record = $this->calls[count($this->calls) - 1];
                $this->assertSame(rtrim($url, '/') . '/test-bucket/', (string) $record['request']->getUri());
                $this->assertEquals(10, $record['options']['timeout']);
                $this->assertFalse($record['options']['verify']);
                $this->assertSame('http://proxy.example:8888', $record['options']['proxy']['https']);
            }
        });
    }

    public function testXmlErrorAndInvalidUrlsDoNotUpload()
    {
        $this->withTransport(403, function () {
            $client = new PrivateLinkProductClient(new Config());
            try {
                $client->upload('http://pvl.example');
                $this->fail('应抛出 OSS 错误');
            } catch (DaraUnableRetryException $wrapper) {
                $error = $wrapper->getLastException();
                $this->assertInstanceOf(ClientException::class, $error);
                $this->assertSame($error, $wrapper->getPrevious());
                $this->assertSame('denied', $wrapper->getMessage());
                $this->assertSame('AccessDenied', $error->code);
                $this->assertSame('denied', $error->message);
                $this->assertSame(403, $error->statusCode);
                $this->assertSame(403, $error->data['httpCode']);
                $this->assertSame('req-id', $error->data['requestId']);
                $this->assertSame('host-id', $error->data['hostId']);
            }
            foreach (array(null, '', 'example.com', '//example.com', 'ftp://example.com',
                'https:///prefix', 'http://', 'http://example.com:99999',
                'http://user@example.com', 'http://user:password@example.com', 'http://@example.com',
                'http://example.com/?a=b', 'http://example.com?', 'http://example.com/#fragment', 'http://example.com#') as $url) {
                try {
                    $client->upload($url);
                    $this->fail('非法地址不应上传');
                } catch (DaraUnableRetryException $wrapper) {
                    $error = $wrapper->getLastException();
                    $this->assertInstanceOf(ClientException::class, $error);
                    $this->assertSame('InvalidOSSPrivateUrl', $error->code, var_export($url, true));
                    $this->assertNull($wrapper->getLastRequest());
                }
            }
            $this->assertCount(1, $this->calls);
        });
    }
}
