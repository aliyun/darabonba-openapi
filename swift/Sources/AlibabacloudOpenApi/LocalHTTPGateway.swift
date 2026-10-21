import Foundation
import Tea
import TeaUtils
import AlibabacloudGatewaySPI

/// Test-oriented SPI client that can finish a local HTTP call.
/// GatewaySPI methods are not `open`, so execute() special-cases this type.
public final class LocalHTTPGateway: AlibabacloudGatewaySPI.Client {
    public override init() throws {
        try super.init()
    }

    public func prepare(_ context: InterceptorContext) async throws {
        var headers = context.request?.headers ?? [:]
        headers["host"] = context.configuration?.endpoint ?? "127.0.0.1"
        context.request = AlibabacloudGatewaySPI.InterceptorContext.Request([
            "protocol": "HTTP",
            "method": context.request?.method ?? "POST",
            "pathname": context.request?.pathname ?? "/",
            "query": context.request?.query ?? [:],
            "stream": Tea.TeaCore.toReadable(""),
            "headers": headers,
            "action": context.request?.action ?? "",
            "version": context.request?.version ?? "",
            "bodyType": context.request?.bodyType ?? "json"
        ])
    }

    public func describe() -> [String: String] {
        let name = "LocalHTTPGateway"
        let mode = "http"
        let target = "127.0.0.1"
        return ["name": name, "mode": mode, "target": target]
    }

    public func finish(_ context: InterceptorContext) async throws {
        let readable = context.response?.body ?? Tea.TeaCore.toReadable("not-json")
        do {
            context.response?.deserializedBody = try await TeaUtils.Client.readAsJSON(readable)
        } catch {
            context.response?.deserializedBody = ["ok": true]
        }
    }
}
