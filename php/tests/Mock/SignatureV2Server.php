<?php

$server = stream_socket_server('tcp://127.0.0.1:0', $errno, $error);
if (!$server) exit(1);
echo substr(strrchr(stream_socket_get_name($server, false), ':'), 1) . "\n";
flush();
while ($client = @stream_socket_accept($server, 30)) {
    $requestLine = trim(fgets($client));
    $headers = [];
    while (($line = fgets($client)) !== false && trim($line) !== '') {
        $parts = explode(':', $line, 2);
        if (count($parts) === 2) $headers[strtolower(trim($parts[0]))] = trim($parts[1]);
    }
    $length = isset($headers['content-length']) ? (int)$headers['content-length'] : 0;
    $body = '';
    while (strlen($body) < $length && !feof($client)) $body .= fread($client, $length - strlen($body));
    $parts = explode(' ', $requestLine);
    $response = json_encode(['method' => $parts[0], 'path' => $parts[1], 'headers' => $headers, 'body' => $body]);
    fwrite($client, "HTTP/1.1 200 OK\r\nContent-Type: application/json\r\nContent-Length: " . strlen($response) . "\r\nConnection: close\r\n\r\n" . $response);
    fclose($client);
}
