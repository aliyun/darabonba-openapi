import Foundation
import Tea
import TeaFileForm

// Stage the multipart body in a private file so Foundation can upload a native
// InputStream and each transport attempt can reopen it without rereading the file field.
func writeOSSPrivateMultipartForm(_ form: [String: Any], _ boundary: String, to url: URL) throws -> Int {
    guard let output = OutputStream(url: url, append: false) else {
        throw Tea.TeaError("Could not create the OSS multipart body")
    }
    output.open()
    defer { output.close() }

    var length = 0
    func write(_ data: Data) throws {
        try data.withUnsafeBytes { (buffer: UnsafeRawBufferPointer) in
            guard let bytes = buffer.bindMemory(to: UInt8.self).baseAddress else { return }
            var offset = 0
            while offset < data.count {
                let count = output.write(bytes.advanced(by: offset), maxLength: data.count - offset)
                guard count > 0 else {
                    throw output.streamError ?? Tea.TeaError("Could not write the OSS multipart body")
                }
                offset += count
            }
        }
        length += data.count
    }

    let fields = form.sorted { $0.key < $1.key }
    let ordinaryFields = fields.filter { $0.key != "file" && !($0.value is TeaFileForm.FileField) }
    let fileFields = fields.filter { $0.key == "file" || $0.value is TeaFileForm.FileField }
    for (name, value) in ordinaryFields + fileFields {
        if let file = value as? TeaFileForm.FileField {
            guard let filename = file.filename, let contentType = file.contentType, let input = file.content,
                  contentType.rangeOfCharacter(from: .newlines) == nil else {
                throw Tea.ValidateError("An OSS file field requires a filename, content type, and content stream")
            }
            try write(Data("--\(boundary)\r\nContent-Disposition: form-data; name=\"file\"; filename=\"\(multipartQuotedValue(filename))\"\r\nContent-Type: \(contentType)\r\n\r\n".utf8))
            input.open()
            defer { input.close() }
            var buffer = [UInt8](repeating: 0, count: 64 * 1024)
            while true {
                let count = input.read(&buffer, maxLength: buffer.count)
                guard count >= 0 else {
                    throw input.streamError ?? Tea.TeaError("Could not read the OSS file field")
                }
                if count == 0 { break }
                try write(Data(buffer.prefix(count)))
            }
            try write(Data("\r\n".utf8))
        } else {
            try write(Data("--\(boundary)\r\nContent-Disposition: form-data; name=\"\(multipartQuotedValue(name))\"\r\n\r\n\(value)\r\n".utf8))
        }
    }
    try write(Data("--\(boundary)--\r\n".utf8))
    return length
}

private func multipartQuotedValue(_ value: String) -> String {
    return value.replacingOccurrences(of: "\\", with: "\\\\")
        .replacingOccurrences(of: "\"", with: "%22")
        .replacingOccurrences(of: "\r", with: "%0D")
        .replacingOccurrences(of: "\n", with: "%0A")
}
