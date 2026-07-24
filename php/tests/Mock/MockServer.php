<?php

$server = @stream_socket_server("tcp://127.0.0.1:8000", $errno, $errstr);

if (!$server) {
    die("Error: $errstr ($errno)");
}

function send_sse($client, $id, $event, $data, $retry = 3000)
{
    $sseData = "id: $id\n";
    $sseData .= "event: $event\n";
    $sseData .= "data: " . json_encode($data) . "\n";
    $sseData .= "retry: $retry\n\n";

    fwrite($client, $sseData);
    fflush($client);
}

function read_http_request($client)
{
    stream_set_timeout($client, 2);
    $request = '';
    while (!feof($client)) {
        $chunk = fread($client, 1024);
        if ($chunk === false || $chunk === '') {
            break;
        }
        $request .= $chunk;
        if (strpos($request, "\r\n\r\n") !== false) {
            break;
        }
        // Guard against oversized / stalled probes.
        if (strlen($request) > 8192) {
            break;
        }
        $meta = stream_get_meta_data($client);
        if (!empty($meta['timed_out'])) {
            break;
        }
    }
    return $request;
}

while (true) {
    $client = @stream_socket_accept($server, 1);
    if ($client === false) {
        // Accept timeout — keep listening for later PHPUnit cases.
        continue;
    }

    $request = read_http_request($client);
    if ($request === '' || $request === false) {
        fclose($client);
        continue;
    }

    $parts = explode("\r\n\r\n", $request, 2);
    $headers = $parts[0];
    $body = isset($parts[1]) ? $parts[1] : '';

    $headerLines = explode("\r\n", $headers);
    $requestLine = array_shift($headerLines);
    if (!is_string($requestLine) || $requestLine === '') {
        fclose($client);
        continue;
    }

    preg_match('/\btimeout:\s*true\b/i', $headers, $timeoutMatch);
    preg_match('/\b(bodytype):[ ]*([\w\d]+)\b/i', $headers, $bodyTypeMatch);
    $bodyType = isset($bodyTypeMatch[2]) ? $bodyTypeMatch[2] : null;
    preg_match('/^(GET|POST|PUT|DELETE)\s(\/\S*)\sHTTP\/1\.[01]/i', $requestLine, $matches);
    if (count($matches) < 3) {
        fwrite($client, "HTTP/1.1 400 Bad Request\r\nConnection: close\r\nContent-Length: 0\r\n\r\n");
        fclose($client);
        continue;
    }
    $method = $matches[1];
    $path = $matches[2];

    // Non-destructive readiness probe used by PHPUnit before first request.
    if ($path === '/health') {
        fwrite($client, "HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\nContent-Length: 2\r\nConnection: close\r\n\r\nOK");
        fclose($client);
        continue;
    }

    $headerAssoc = [];
    foreach ($headerLines as $header) {
        if (strpos($header, ':') === false) {
            continue;
        }
        $headerParts = explode(':', $header, 2);
        $name = strtolower(trim($headerParts[0]));
        $value = isset($headerParts[1]) ? trim($headerParts[1]) : '';
        $headerAssoc[$name] = $value;
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
        fflush($client);

        // Send all SSE events immediately — inter-event sleep caused flakes on
        // PHP 5.6/7.0 + Guzzle 6 (stream readTimeout / partial event frames).
        for ($count = 0; $count < 5; $count++) {
            send_sse($client, 'sse-test', 'flow', array('count' => $count), 3000);
        }
        fclose($client);
        continue;
    }

    if ($timeoutMatch) {
        sleep(5);
        $responseHeaders = "HTTP/1.1 500 Internal Server Error\r\n" .
            "Content-Type: text/plain\r\n" .
            "Connection: close\r\n\r\n";
        fwrite($client, $responseHeaders . "Server Timeout");
    } else {
        $responseHeaders = "HTTP/1.1 200 OK\r\n" .
            "Connection: close\r\n" .
            "Content-Type: application/json\r\n" .
            "x-acs-request-id: A45EE076-334D-5012-9746-A8F828D20FD4\r\n" .
            "http-method: $method\r\n" .
            "pathname: $path\r\n" .
            "raw-body: $body\r\n";

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
