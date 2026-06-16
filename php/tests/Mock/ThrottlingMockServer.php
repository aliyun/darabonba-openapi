<?php

$stateFile = __DIR__ . '/throttling-mock-state.json';
$server = stream_socket_server('tcp://0.0.0.0:8001', $errno, $errstr);

if (!$server) {
    die("Error: $errstr ($errno)\n");
}

function readState($stateFile)
{
    if (!file_exists($stateFile)) {
        return [
            'throttleCount' => 2,
            'retryAfterMS' => 1,
            'requestCount' => 0,
            'retryAttempts' => [],
            'retryDelays' => [],
        ];
    }

    $state = json_decode(file_get_contents($stateFile), true);
    if (!is_array($state)) {
        return [
            'throttleCount' => 2,
            'retryAfterMS' => 1,
            'requestCount' => 0,
            'retryAttempts' => [],
            'retryDelays' => [],
        ];
    }

    return $state;
}

function writeState($stateFile, $state)
{
    file_put_contents($stateFile, json_encode($state));
}

while ($client = @stream_socket_accept($server)) {
    $request = fread($client, 65536);
    list($headers, $body) = array_pad(explode("\r\n\r\n", $request, 2), 2, '');
    $headerLines = explode("\r\n", $headers);
    $requestLine = array_shift($headerLines);
    $headerAssoc = [];

    foreach ($headerLines as $headerLine) {
        if (strpos($headerLine, ': ') === false) {
            continue;
        }
        list($key, $value) = explode(': ', $headerLine, 2);
        $headerAssoc[strtolower($key)] = trim($value);
    }

    $state = readState($stateFile);
    $state['requestCount'] = isset($state['requestCount']) ? $state['requestCount'] + 1 : 1;
    $state['retryAttempts'][] = isset($headerAssoc['x-acs-retry-attempts']) ? $headerAssoc['x-acs-retry-attempts'] : '';
    $state['retryDelays'][] = isset($headerAssoc['x-acs-retry-delay']) ? $headerAssoc['x-acs-retry-delay'] : '';
    writeState($stateFile, $state);

    $throttleCount = isset($state['throttleCount']) ? (int)$state['throttleCount'] : 2;
    $retryAfterMS = isset($state['retryAfterMS']) ? (int)$state['retryAfterMS'] : 1;

    if ($state['requestCount'] <= $throttleCount) {
        $responseBody = '{"Code":"Throttling","Message":"Request was denied due to user flow control.","RequestId":"A45EE076-334D-5012-9746-A8F828D20FD4"}';
        $statusLine = 'HTTP/1.1 400 Bad Request';
    } else {
        $responseBody = '{"RequestId":"A45EE076-334D-5012-9746-A8F828D20FD4","Quotas":[]}';
        $statusLine = 'HTTP/1.1 200 OK';
    }

    $responseHeaders = $statusLine . "\r\n" .
        "Connection: close\r\n" .
        "Content-Type: application/json\r\n" .
        "x-acs-request-id: A45EE076-334D-5012-9746-A8F828D20FD4\r\n" .
        "raw-body: $body\r\n";

    foreach ($headerAssoc as $name => $value) {
        if ($name === 'content-length' || $name === 'host') {
            continue;
        }
        $responseHeaders .= "$name: $value\r\n";
    }

    if ($state['requestCount'] <= $throttleCount) {
        $responseHeaders .= "x-acs-retry-after: $retryAfterMS\r\n";
    }

    $responseHeaders .= "\r\n";

    fwrite($client, $responseHeaders . $responseBody);
    fclose($client);
}
