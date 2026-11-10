import Foundation
import XCTest
#if os(Linux)
import Glibc
#else
import Darwin
#endif

final class HTTPTestServerTests: XCTestCase {
    func testReadsFragmentedRequestBodies() throws {
        let cases = [
            ("Content-Length: 5", "he", "llo", "hello"),
            ("Transfer-Encoding: chunked", "3\r\nab", "c\r\n2\r\nde\r\n0\r\n\r\n", "3\r\nabc\r\n2\r\nde\r\n0\r\n\r\n")
        ]
        for (header, prefix, suffix, expected) in cases {
            let server = try HTTPTestServer()
            defer { server.stop() }
            #if os(Linux)
            let fd = socket(AF_INET, Int32(SOCK_STREAM.rawValue), 0)
            #else
            let fd = socket(AF_INET, SOCK_STREAM, 0)
            #endif
            XCTAssertGreaterThanOrEqual(fd, 0)
            guard fd >= 0 else { return }
            defer { close(fd) }
            #if !os(Linux)
            var noSignal: Int32 = 1
            setsockopt(fd, SOL_SOCKET, SO_NOSIGPIPE, &noSignal, socklen_t(MemoryLayout<Int32>.size))
            #endif
            var address = sockaddr_in()
            address.sin_family = sa_family_t(AF_INET)
            address.sin_addr.s_addr = inet_addr("127.0.0.1")
            address.sin_port = UInt16(server.port).bigEndian
            let connected = withUnsafePointer(to: &address) {
                $0.withMemoryRebound(to: sockaddr.self, capacity: 1) {
                    connect(fd, $0, socklen_t(MemoryLayout<sockaddr_in>.size))
                }
            }
            XCTAssertEqual(connected, 0)
            guard connected == 0 else { return }
            func sendAll(_ text: String) -> Bool {
                let bytes = Array(text.utf8)
                return bytes.withUnsafeBytes { raw in
                    var sent = 0
                    while sent < raw.count {
                        #if os(Linux)
                        let count = send(fd, raw.baseAddress!.advanced(by: sent), raw.count - sent, Int32(MSG_NOSIGNAL))
                        #else
                        let count = send(fd, raw.baseAddress!.advanced(by: sent), raw.count - sent, 0)
                        #endif
                        guard count > 0 else { return false }
                        sent += count
                    }
                    return true
                }
            }
            XCTAssertTrue(sendAll("POST / HTTP/1.1\r\nHost: localhost\r\n\(header)\r\n\r\n" + prefix))
            var item = pollfd(fd: fd, events: Int16(POLLIN), revents: 0)
            let ready = poll(&item, 1, 200)
            XCTAssertEqual(ready, 0, "server responded before receiving the complete body")
            guard ready == 0, sendAll(suffix) else { return }
            var timeout = timeval(tv_sec: 3, tv_usec: 0)
            setsockopt(fd, SOL_SOCKET, SO_RCVTIMEO, &timeout, socklen_t(MemoryLayout<timeval>.size))
            var response = Data()
            var buffer = [UInt8](repeating: 0, count: 4096)
            while true {
                let count = recv(fd, &buffer, buffer.count, 0)
                if count <= 0 { break }
                response.append(contentsOf: buffer.prefix(count))
            }
            let text = String(decoding: response, as: UTF8.self)
            XCTAssertTrue(text.hasPrefix("HTTP/1.1 200 OK\r\n"))
            XCTAssertTrue(text.hasSuffix(expected))
        }
    }
}
