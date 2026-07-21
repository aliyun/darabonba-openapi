<?php

$server = stream_socket_server("tcp://0.0.0.0:8000", $errno, $errstr);

if (!$server) {
    die("Error: $errstr ($errno)");
}

function send_sse($client, $id, $event, $data, $retry = 3000)
{
    $sseData = "id: $id\n";
    $sseData .= "event: $event\n";
    $sseData .= "data: " . json_encode($data) . "\n";
    $sseData .= "retry: $retry\n\n";  // The retry comment is optional and should follow client's needs

    fwrite($client, $sseData);
    fflush($client);
}

while ($client = @stream_socket_accept($server)) {
    // 读取请求
    $request = fread($client, 1024);
    list($headers, $body) = explode("\r\n\r\n", $request, 2);
    // 简单解析请求头
    $headerLines = explode("\r\n", $headers);
    $requestLine = array_shift($headerLines);
    preg_match('/\btimeout:\s*true\b/i', $headers, $timeoutMatch);
    preg_match('/\b(bodytype):[ ]*([\w\d]+)\b/i', $headers, $bodyTypeMatch);
    $bodyType = isset($bodyTypeMatch[2]) ? $bodyTypeMatch[2] : null;
    preg_match('/^(GET|POST|PUT|DELETE)\s(\/\S*)\sHTTP\/1.1/', $requestLine, $matches);
    $method = $matches[1];
    $path = $matches[2];


    $headerAssoc = [];

    foreach ($headerLines as $header) {
        list($name, $value) = explode(": ", $header, 2);
        $headerAssoc[strtolower($name)] = $value;
    }
    if (substr($path, 0, 4) === '/sse') {
        $responseHeaders = "HTTP/1.1 200 OK\r\n" .
            "Content-Type: text/event-stream;charset=UTF-8\r\n" .
            "Cache-Control: no-cache\r\n" .
            "Connection: keep-alive\r\n";
        foreach ($headerAssoc as $name => $value) {
            $responseHeaders .= $name . ": " . $value . "\r\n";
        }
        $responseHeaders .= "\r\n";
        fwrite($client, $responseHeaders);
        // fflush the socket (flush() only clears PHP's output buffer)
        fflush($client);

        // 模拟发送事件流 — keep inter-event delay small so CI readTimeout
        // (esp. PHP 5.6 + Guzzle 6) does not flake mid-stream.
        $count = 0;
        while ($count < 5) {
            $data = [
                "count" => $count
            ];
            send_sse($client, 'sse-test', 'flow', $data, 3000);
            usleep(10000);
            $count++;
        }
        fclose($client);
        continue;
    }

    if ($timeoutMatch) {
        // 模拟超时
        sleep(5);
        $responseHeaders = "HTTP/1.1 500 Internal Server Error\r\n" .
            "Content-Type: text/plain\r\n" .
            "Connection: close\r\n\r\n";
        fwrite($client, $responseHeaders . "Server Timeout");
    } else {
        $headerAssoc = [];
        foreach ($headerLines as $headerLine) {
            list($key, $value) = explode(': ', $headerLine, 2);
            $headerAssoc[strtolower($key)] = trim($value);
        }

        // 获取路径和请求方法
        preg_match('/^(GET|POST|PUT|DELETE)\s(\/\S*)\sHTTP\/1.1/', $requestLine, $matches);
        $method = $matches[1];
        $path = $matches[2];

        // 构建响应头
        $responseHeaders = "HTTP/1.1 200 OK\r\n" .
            "Connection: close\r\n" .
            "Content-Type: application/json\r\n" .
            "x-acs-request-id: A45EE076-334D-5012-9746-A8F828D20FD4\r\n" .
            "http-method: $method\r\n" .
            "pathname: $path\r\n" .
            "raw-body: $body\r\n";

        // 构建响应体
        $responseBody = "";

        echo $bodyType . "\n";

        switch ($bodyType) {
            case 'array':
                $responseBody = json_encode(["AppId", "ClassId", "UserId"]);
                break;
            case 'error':
                $responseHeaders = "HTTP/1.1 400 Bad Request\r\n" .
                    "Connection: close\r\n" .
                    "Content-Type: application/json\r\n" .
                    "x-acs-request-id: A45EE076-334D-5012-9746-A8F828D20FD4\r\n" .
                    "http-method: $method\r\n" .
                    "pathname: $path\r\n";
                $responseBody = json_encode([
                    "Code" => "error code",
                    "Message" => "error message",
                    "RequestId" => "A45EE076-334D-5012-9746-A8F828D20FD4",
                    "Description" => "error description",
                    "AccessDeniedDetail" => new stdClass()
                ]);
                break;
            case 'error1':
                $responseHeaders = "HTTP/1.1 400 Bad Request\r\n" .
                    "Connection: close\r\n" .
                    "Content-Type: application/json\r\n" .
                    "x-acs-request-id: A45EE076-334D-5012-9746-A8F828D20FD4\r\n" .
                    "http-method: $method\r\n" .
                    "pathname: $path\r\n";
                $responseBody = json_encode([
                    "Code" => "error code",
                    "Message" => "error message",
                    "RequestId" => "A45EE076-334D-5012-9746-A8F828D20FD4",
                    "Description" => "error description",
                    "AccessDeniedDetail" => new stdClass(),
                    "accessDeniedDetail" => ["test" => 0]
                ]);
                break;
            case 'error2':
                $responseHeaders = "HTTP/1.1 400 Bad Request\r\n" .
                    "Connection: close\r\n" .
                    "Content-Type: application/json\r\n" .
                    "x-acs-request-id: A45EE076-334D-5012-9746-A8F828D20FD4\r\n" .
                    "http-method: $method\r\n" .
                    "pathname: $path\r\n";
                $responseBody = json_encode([
                    "Code" => "error code",
                    "Message" => "error message",
                    "RequestId" => "A45EE076-334D-5012-9746-A8F828D20FD4",
                    "Description" => "error description",
                    "accessDeniedDetail" => ["test" => 0]
                ]);
                break;
            default:
                $responseBody = json_encode([
                    "AppId" => "test",
                    "ClassId" => "test",
                    "UserId" => 123
                ]);
        }

        fwrite($client, $responseHeaders . "\r\n" . $responseBody);
    }
    fclose($client);
    continue;
}

fclose($server);
