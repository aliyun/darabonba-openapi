import Darwin
import Foundation

final class TinyHTTPServer {
    private(set) var port: UInt16 = 0
    private var listenFD: Int32 = -1
    private var running = false
    private let queue = DispatchQueue(label: "tiny-http-server", attributes: .concurrent)

    init() {
        listenFD = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP)
        var reuse: Int32 = 1
        setsockopt(listenFD, SOL_SOCKET, SO_REUSEADDR, &reuse, socklen_t(MemoryLayout<Int32>.size))
        var nosig: Int32 = 1
        setsockopt(listenFD, SOL_SOCKET, SO_NOSIGPIPE, &nosig, socklen_t(MemoryLayout<Int32>.size))
        var addr = sockaddr_in()
        addr.sin_len = UInt8(MemoryLayout<sockaddr_in>.size)
        addr.sin_family = sa_family_t(AF_INET)
        addr.sin_addr.s_addr = inet_addr("127.0.0.1")
        addr.sin_port = 0
        withUnsafePointer(to: &addr) { ptr in
            ptr.withMemoryRebound(to: sockaddr.self, capacity: 1) {
                _ = bind(self.listenFD, $0, socklen_t(MemoryLayout<sockaddr_in>.size))
            }
        }
        listen(listenFD, 32)
        var bound = sockaddr_in()
        var len = socklen_t(MemoryLayout<sockaddr_in>.size)
        withUnsafeMutablePointer(to: &bound) { ptr in
            ptr.withMemoryRebound(to: sockaddr.self, capacity: 1) {
                _ = getsockname(self.listenFD, $0, &len)
            }
        }
        port = UInt16(bigEndian: bound.sin_port)
        running = true
        queue.async { [weak self] in
            self?.acceptLoop()
        }
        waitUntilAccepting()
    }

    deinit {
        stop()
    }

    func stop() {
        running = false
        if listenFD >= 0 {
            close(listenFD)
            listenFD = -1
        }
    }

    private func waitUntilAccepting() {
        for _ in 0..<50 {
            let fd = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP)
            var addr = sockaddr_in()
            addr.sin_len = UInt8(MemoryLayout<sockaddr_in>.size)
            addr.sin_family = sa_family_t(AF_INET)
            addr.sin_addr.s_addr = inet_addr("127.0.0.1")
            addr.sin_port = port.bigEndian
            let rc = withUnsafePointer(to: &addr) { ptr -> Int32 in
                ptr.withMemoryRebound(to: sockaddr.self, capacity: 1) {
                    connect(fd, $0, socklen_t(MemoryLayout<sockaddr_in>.size))
                }
            }
            close(fd)
            if rc == 0 {
                return
            }
            usleep(20_000)
        }
    }

    private func acceptLoop() {
        while running {
            var addr = sockaddr_in()
            var len = socklen_t(MemoryLayout<sockaddr_in>.size)
            let client = withUnsafeMutablePointer(to: &addr) { ptr -> Int32 in
                ptr.withMemoryRebound(to: sockaddr.self, capacity: 1) {
                    accept(self.listenFD, $0, &len)
                }
            }
            if client < 0 {
                continue
            }
            var nosig: Int32 = 1
            setsockopt(client, SOL_SOCKET, SO_NOSIGPIPE, &nosig, socklen_t(MemoryLayout<Int32>.size))
            queue.async {
                self.handle(client)
            }
        }
    }

    private func handle(_ client: Int32) {
        let request = readRequest(client)
        let firstLine = request.split(separator: "\r\n").first.map(String.init) ?? ""
        let path = firstLine.split(separator: " ").dropFirst().first.map(String.init) ?? "/"
        var status = "200 OK"
        var contentType = "application/json"
        var body = "{\"AppId\":\"test\",\"ClassId\":\"test\",\"UserId\":123}"
        if path.hasPrefix("/empty") {
            status = "204 No Content"
            body = ""
        } else if path.hasPrefix("/xml") {
            status = "400 Bad Request"
            contentType = "text/xml;charset=utf-8"
            body = "<?xml version=\"1.0\"?><Error><Code>Invalid</Code><Message>xml error</Message><RequestId>xml-id</RequestId></Error>"
        } else if path.hasPrefix("/server") {
            status = "500 Internal Server Error"
            body = "{\"Code\":\"InternalError\",\"Message\":\"boom\",\"RequestId\":\"rid\"}"
        } else if path.hasPrefix("/array") {
            body = "[\"AppId\",\"ClassId\",\"UserId\"]"
        } else if path.hasPrefix("/error") {
            status = "400 Bad Request"
            body = "{\"Code\":\"error code\",\"Message\":\"error message\",\"RequestId\":\"xml-id\",\"Description\":\"desc\",\"AccessDeniedDetail\":{}}"
        }
        var response = "HTTP/1.1 \(status)\r\nConnection: close\r\nContent-Type: \(contentType)\r\nx-acs-request-id: A45EE076-334D-5012-9746-A8F828D20FD4\r\n"
        if body.isEmpty {
            response += "Content-Length: 0\r\n\r\n"
        } else {
            response += "Content-Length: \(body.utf8.count)\r\n\r\n\(body)"
        }
        let bytes = Array(response.utf8)
        bytes.withUnsafeBytes { ptr in
            guard let base = ptr.baseAddress else { return }
            _ = send(client, base, bytes.count, 0)
        }
        close(client)
    }

    private func readRequest(_ client: Int32) -> String {
        var data = Data()
        var buffer = [UInt8](repeating: 0, count: 2048)
        var tv = timeval(tv_sec: 2, tv_usec: 0)
        setsockopt(client, SOL_SOCKET, SO_RCVTIMEO, &tv, socklen_t(MemoryLayout<timeval>.size))
        while data.count < 64 * 1024 {
            let n = recv(client, &buffer, buffer.count, 0)
            if n <= 0 {
                break
            }
            data.append(contentsOf: buffer[0..<n])
            guard let text = String(data: data, encoding: .utf8),
                  let headerRange = text.range(of: "\r\n\r\n") else {
                continue
            }
            let headerText = String(text[..<headerRange.lowerBound])
            var contentLength = 0
            for line in headerText.split(separator: "\r\n") {
                let parts = line.split(separator: ":", maxSplits: 1)
                if parts.count == 2 && parts[0].lowercased() == "content-length" {
                    contentLength = Int(parts[1].trimmingCharacters(in: .whitespaces)) ?? 0
                }
            }
            let headerBytes = text.distance(from: text.startIndex, to: headerRange.upperBound)
            if data.count >= headerBytes + contentLength {
                break
            }
        }
        return String(data: data, encoding: .utf8) ?? ""
    }
}
