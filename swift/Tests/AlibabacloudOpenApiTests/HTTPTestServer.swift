import Foundation
#if os(Linux)
import Glibc
#else
import Darwin
#endif

// 支持一个 HTTP 请求及完整 Content-Length/chunked 请求体。
// 未指定响应体时回显收到的请求。
final class HTTPTestServer {
    let port: Int
    private var listener: Int32
    private let finished = DispatchGroup()

    init(body: String? = nil, statusCode: Int = 200, contentType: String = "application/json",
         onRequest: ((String, Data) -> Void)? = nil) throws {
        #if os(Linux)
        let fd = socket(AF_INET, Int32(SOCK_STREAM.rawValue), 0)
        #else
        let fd = socket(AF_INET, SOCK_STREAM, 0)
        #endif
        guard fd >= 0 else { throw POSIXError(.EIO) }
        var address = sockaddr_in()
        address.sin_family = sa_family_t(AF_INET)
        address.sin_addr.s_addr = inet_addr("127.0.0.1")
        var length = socklen_t(MemoryLayout<sockaddr_in>.size)
        let bound = withUnsafePointer(to: &address) {
            $0.withMemoryRebound(to: sockaddr.self, capacity: 1) { bind(fd, $0, length) }
        }
        guard bound == 0, listen(fd, 1) == 0 else {
            close(fd)
            throw POSIXError(.EADDRINUSE)
        }
        let named = withUnsafeMutablePointer(to: &address) {
            $0.withMemoryRebound(to: sockaddr.self, capacity: 1) { getsockname(fd, $0, &length) }
        }
        guard named == 0 else {
            close(fd)
            throw POSIXError(.EIO)
        }
        port = Int(UInt16(bigEndian: address.sin_port))
        listener = fd
        let finished = self.finished
        finished.enter()
        DispatchQueue.global().async {
            defer { finished.leave() }
            let connection = accept(fd, nil, nil)
            guard connection >= 0 else { return }
            defer { close(connection) }
            var timeout = timeval(tv_sec: 3, tv_usec: 0)
            setsockopt(connection, SOL_SOCKET, SO_RCVTIMEO, &timeout, socklen_t(MemoryLayout<timeval>.size))
            #if !os(Linux)
            var noSignal: Int32 = 1
            setsockopt(connection, SOL_SOCKET, SO_NOSIGPIPE, &noSignal, socklen_t(MemoryLayout<Int32>.size))
            #endif
            var request = Data()
            var buffer = [UInt8](repeating: 0, count: 4096)
            while request.count < 1_048_576 {
                let count = recv(connection, &buffer, buffer.count, 0)
                guard count > 0 else { return }
                request.append(contentsOf: buffer.prefix(count))
                if let (headers, content) = Self.parseRequest(request) {
                    onRequest?(headers, content)
                    break
                }
            }
            let payload = body.map { Data($0.utf8) } ?? request
            var response = Data("HTTP/1.1 \(statusCode) Mock\r\nContent-Length: \(payload.count)\r\nContent-Type: \(contentType)\r\nX-Mock: tea\r\nConnection: close\r\n\r\n".utf8)
            response.append(payload)
            response.withUnsafeBytes { bytes in
                var sent = 0
                while sent < bytes.count {
                    #if os(Linux)
                    let count = send(connection, bytes.baseAddress!.advanced(by: sent), bytes.count - sent, Int32(MSG_NOSIGNAL))
                    #else
                    let count = send(connection, bytes.baseAddress!.advanced(by: sent), bytes.count - sent, 0)
                    #endif
                    guard count > 0 else { return }
                    sent += count
                }
            }
        }
    }

    private static func parseRequest(_ request: Data) -> (String, Data)? {
        let separator = Data("\r\n\r\n".utf8)
        guard let headerEnd = request.range(of: separator),
              let headers = String(data: request[..<headerEnd.lowerBound], encoding: .utf8) else { return nil }
        let fields = headers.components(separatedBy: "\r\n").dropFirst()
        let chunked = fields.contains { $0.lowercased().hasPrefix("transfer-encoding:") && $0.lowercased().contains("chunked") }
        if !chunked {
            let length = fields.first { $0.lowercased().hasPrefix("content-length:") }
                .flatMap { Int($0.split(separator: ":", maxSplits: 1)[1].trimmingCharacters(in: .whitespaces)) } ?? 0
            guard request.count >= headerEnd.upperBound + length else { return nil }
            return (headers, Data(request[headerEnd.upperBound..<(headerEnd.upperBound + length)]))
        }
        var cursor = headerEnd.upperBound
        var content = Data()
        let crlf = Data("\r\n".utf8)
        while cursor < request.count {
            guard let end = request.range(of: crlf, in: cursor..<request.count),
                  let line = String(data: request[cursor..<end.lowerBound], encoding: .utf8),
                  let length = Int(line.components(separatedBy: ";")[0], radix: 16) else { return nil }
            cursor = end.upperBound
            guard request.count >= cursor + length + 2 else { return nil }
            if length == 0 { return (headers, content) }
            content.append(request[cursor..<(cursor + length)])
            cursor += length + 2
        }
        return nil
    }

    func stop() {
        guard listener >= 0 else { return }
        shutdown(listener, Int32(SHUT_RDWR))
        close(listener)
        listener = -1
        finished.wait()
    }

    deinit { stop() }
}
