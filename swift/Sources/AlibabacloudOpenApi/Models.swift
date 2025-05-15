import Foundation
import Tea
import TeaUtils
import AlibabaCloudCredentials
import AlibabaCloudOpenApiUtil
import AlibabacloudGatewaySPI
import DarabonbaXML

public class GlobalParameters : Tea.TeaModel {
    public var headers: [String: String]?

    public var queries: [String: String]?

    public override init() {
        super.init()
    }

    public init(_ dict: [String: Any]) {
        super.init()
        self.fromMap(dict)
    }

    public override func validate() throws -> Void {
    }

    public override func toMap() -> [String : Any] {
        var map = super.toMap()
        if self.headers != nil {
            map["headers"] = self.headers!
        }
        if self.queries != nil {
            map["queries"] = self.queries!
        }
        return map
    }

    public override func fromMap(_ dict: [String: Any?]?) -> Void {
        guard let dict else { return }
        if let value = dict["headers"] as? [String: String] {
            self.headers = value
        }
        if let value = dict["queries"] as? [String: String] {
            self.queries = value
        }
    }
}

public class Config : Tea.TeaModel {
    public var accessKeyId: String?

    public var accessKeySecret: String?

    public var securityToken: String?

    public var bearerToken: String?

    public var protocol_: String?

    public var method: String?

    public var regionId: String?

    public var readTimeout: Int?

    public var connectTimeout: Int?

    public var httpProxy: String?

    public var httpsProxy: String?

    public var credential: AlibabaCloudCredentials.Client?

    public var endpoint: String?

    public var noProxy: String?

    public var maxIdleConns: Int?

    public var network: String?

    public var userAgent: String?

    public var suffix: String?

    public var socks5Proxy: String?

    public var socks5NetWork: String?

    public var endpointType: String?

    public var openPlatformEndpoint: String?

    public var type: String?

    public var signatureVersion: String?

    public var signatureAlgorithm: String?

    public var globalParameters: GlobalParameters?

    public var key: String?

    public var cert: String?

    public var ca: String?

    public var disableHttp2: Bool?

    public var tlsMinVersion: String?

    public override init() {
        super.init()
    }

    public init(_ dict: [String: Any]) {
        super.init()
        self.fromMap(dict)
    }

    public override func validate() throws -> Void {
        try self.globalParameters?.validate()
    }

    public override func toMap() -> [String : Any] {
        var map = super.toMap()
        if self.accessKeyId != nil {
            map["accessKeyId"] = self.accessKeyId!
        }
        if self.accessKeySecret != nil {
            map["accessKeySecret"] = self.accessKeySecret!
        }
        if self.securityToken != nil {
            map["securityToken"] = self.securityToken!
        }
        if self.bearerToken != nil {
            map["bearerToken"] = self.bearerToken!
        }
        if self.protocol_ != nil {
            map["protocol"] = self.protocol_!
        }
        if self.method != nil {
            map["method"] = self.method!
        }
        if self.regionId != nil {
            map["regionId"] = self.regionId!
        }
        if self.readTimeout != nil {
            map["readTimeout"] = self.readTimeout!
        }
        if self.connectTimeout != nil {
            map["connectTimeout"] = self.connectTimeout!
        }
        if self.httpProxy != nil {
            map["httpProxy"] = self.httpProxy!
        }
        if self.httpsProxy != nil {
            map["httpsProxy"] = self.httpsProxy!
        }
        if self.credential != nil {
            map["credential"] = self.credential!
        }
        if self.endpoint != nil {
            map["endpoint"] = self.endpoint!
        }
        if self.noProxy != nil {
            map["noProxy"] = self.noProxy!
        }
        if self.maxIdleConns != nil {
            map["maxIdleConns"] = self.maxIdleConns!
        }
        if self.network != nil {
            map["network"] = self.network!
        }
        if self.userAgent != nil {
            map["userAgent"] = self.userAgent!
        }
        if self.suffix != nil {
            map["suffix"] = self.suffix!
        }
        if self.socks5Proxy != nil {
            map["socks5Proxy"] = self.socks5Proxy!
        }
        if self.socks5NetWork != nil {
            map["socks5NetWork"] = self.socks5NetWork!
        }
        if self.endpointType != nil {
            map["endpointType"] = self.endpointType!
        }
        if self.openPlatformEndpoint != nil {
            map["openPlatformEndpoint"] = self.openPlatformEndpoint!
        }
        if self.type != nil {
            map["type"] = self.type!
        }
        if self.signatureVersion != nil {
            map["signatureVersion"] = self.signatureVersion!
        }
        if self.signatureAlgorithm != nil {
            map["signatureAlgorithm"] = self.signatureAlgorithm!
        }
        if self.globalParameters != nil {
            map["globalParameters"] = self.globalParameters?.toMap()
        }
        if self.key != nil {
            map["key"] = self.key!
        }
        if self.cert != nil {
            map["cert"] = self.cert!
        }
        if self.ca != nil {
            map["ca"] = self.ca!
        }
        if self.disableHttp2 != nil {
            map["disableHttp2"] = self.disableHttp2!
        }
        if self.tlsMinVersion != nil {
            map["tlsMinVersion"] = self.tlsMinVersion!
        }
        return map
    }

    public override func fromMap(_ dict: [String: Any?]?) -> Void {
        guard let dict else { return }
        if let value = dict["accessKeyId"] as? String {
            self.accessKeyId = value
        }
        if let value = dict["accessKeySecret"] as? String {
            self.accessKeySecret = value
        }
        if let value = dict["securityToken"] as? String {
            self.securityToken = value
        }
        if let value = dict["bearerToken"] as? String {
            self.bearerToken = value
        }
        if let value = dict["protocol"] as? String {
            self.protocol_ = value
        }
        if let value = dict["method"] as? String {
            self.method = value
        }
        if let value = dict["regionId"] as? String {
            self.regionId = value
        }
        if let value = dict["readTimeout"] as? Int {
            self.readTimeout = value
        }
        if let value = dict["connectTimeout"] as? Int {
            self.connectTimeout = value
        }
        if let value = dict["httpProxy"] as? String {
            self.httpProxy = value
        }
        if let value = dict["httpsProxy"] as? String {
            self.httpsProxy = value
        }
        if let value = dict["credential"] as? AlibabaCloudCredentials.Client {
            self.credential = value
        }
        if let value = dict["endpoint"] as? String {
            self.endpoint = value
        }
        if let value = dict["noProxy"] as? String {
            self.noProxy = value
        }
        if let value = dict["maxIdleConns"] as? Int {
            self.maxIdleConns = value
        }
        if let value = dict["network"] as? String {
            self.network = value
        }
        if let value = dict["userAgent"] as? String {
            self.userAgent = value
        }
        if let value = dict["suffix"] as? String {
            self.suffix = value
        }
        if let value = dict["socks5Proxy"] as? String {
            self.socks5Proxy = value
        }
        if let value = dict["socks5NetWork"] as? String {
            self.socks5NetWork = value
        }
        if let value = dict["endpointType"] as? String {
            self.endpointType = value
        }
        if let value = dict["openPlatformEndpoint"] as? String {
            self.openPlatformEndpoint = value
        }
        if let value = dict["type"] as? String {
            self.type = value
        }
        if let value = dict["signatureVersion"] as? String {
            self.signatureVersion = value
        }
        if let value = dict["signatureAlgorithm"] as? String {
            self.signatureAlgorithm = value
        }
        if let value = dict["globalParameters"] as? [String: Any?] {
            var model = GlobalParameters()
            model.fromMap(value)
            self.globalParameters = model
        }
        if let value = dict["key"] as? String {
            self.key = value
        }
        if let value = dict["cert"] as? String {
            self.cert = value
        }
        if let value = dict["ca"] as? String {
            self.ca = value
        }
        if let value = dict["disableHttp2"] as? Bool {
            self.disableHttp2 = value
        }
        if let value = dict["tlsMinVersion"] as? String {
            self.tlsMinVersion = value
        }
    }
}

public class OpenApiRequest : Tea.TeaModel {
    public var headers: [String: String]?

    public var query: [String: String]?

    public var body: Any?

    public var stream: InputStream?

    public var hostMap: [String: String]?

    public var endpointOverride: String?

    public override init() {
        super.init()
    }

    public init(_ dict: [String: Any]) {
        super.init()
        self.fromMap(dict)
    }

    public override func validate() throws -> Void {
    }

    public override func toMap() -> [String : Any] {
        var map = super.toMap()
        if self.headers != nil {
            map["headers"] = self.headers!
        }
        if self.query != nil {
            map["query"] = self.query!
        }
        if self.body != nil {
            map["body"] = self.body!
        }
        if self.stream != nil {
            map["stream"] = self.stream!
        }
        if self.hostMap != nil {
            map["hostMap"] = self.hostMap!
        }
        if self.endpointOverride != nil {
            map["endpointOverride"] = self.endpointOverride!
        }
        return map
    }

    public override func fromMap(_ dict: [String: Any?]?) -> Void {
        guard let dict else { return }
        if let value = dict["headers"] as? [String: String] {
            self.headers = value
        }
        if let value = dict["query"] as? [String: String] {
            self.query = value
        }
        if let value = dict["body"] as? Any {
            self.body = value
        }
        if let value = dict["stream"] as? InputStream {
            self.stream = value
        }
        if let value = dict["hostMap"] as? [String: String] {
            self.hostMap = value
        }
        if let value = dict["endpointOverride"] as? String {
            self.endpointOverride = value
        }
    }
}

public class Params : Tea.TeaModel {
    public var action: String?

    public var version: String?

    public var protocol_: String?

    public var pathname: String?

    public var method: String?

    public var authType: String?

    public var bodyType: String?

    public var reqBodyType: String?

    public var style: String?

    public override init() {
        super.init()
    }

    public init(_ dict: [String: Any]) {
        super.init()
        self.fromMap(dict)
    }

    public override func validate() throws -> Void {
        try self.validateRequired(self.action, "action")
        try self.validateRequired(self.version, "version")
        try self.validateRequired(self.protocol_, "protocol_")
        try self.validateRequired(self.pathname, "pathname")
        try self.validateRequired(self.method, "method")
        try self.validateRequired(self.authType, "authType")
        try self.validateRequired(self.bodyType, "bodyType")
        try self.validateRequired(self.reqBodyType, "reqBodyType")
    }

    public override func toMap() -> [String : Any] {
        var map = super.toMap()
        if self.action != nil {
            map["action"] = self.action!
        }
        if self.version != nil {
            map["version"] = self.version!
        }
        if self.protocol_ != nil {
            map["protocol"] = self.protocol_!
        }
        if self.pathname != nil {
            map["pathname"] = self.pathname!
        }
        if self.method != nil {
            map["method"] = self.method!
        }
        if self.authType != nil {
            map["authType"] = self.authType!
        }
        if self.bodyType != nil {
            map["bodyType"] = self.bodyType!
        }
        if self.reqBodyType != nil {
            map["reqBodyType"] = self.reqBodyType!
        }
        if self.style != nil {
            map["style"] = self.style!
        }
        return map
    }

    public override func fromMap(_ dict: [String: Any?]?) -> Void {
        guard let dict else { return }
        if let value = dict["action"] as? String {
            self.action = value
        }
        if let value = dict["version"] as? String {
            self.version = value
        }
        if let value = dict["protocol"] as? String {
            self.protocol_ = value
        }
        if let value = dict["pathname"] as? String {
            self.pathname = value
        }
        if let value = dict["method"] as? String {
            self.method = value
        }
        if let value = dict["authType"] as? String {
            self.authType = value
        }
        if let value = dict["bodyType"] as? String {
            self.bodyType = value
        }
        if let value = dict["reqBodyType"] as? String {
            self.reqBodyType = value
        }
        if let value = dict["style"] as? String {
            self.style = value
        }
    }
}
