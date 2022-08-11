import Foundation
import Tea
import TeaUtils
import AlibabaCloudCredentials
import AlibabaCloudOpenApiUtil

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

    public override func fromMap(_ dict: [String: Any]) -> Void {
        if dict.keys.contains("headers") {
            self.headers = dict["headers"] as! [String: String]
        }
        if dict.keys.contains("queries") {
            self.queries = dict["queries"] as! [String: String]
        }
    }
}

public class Config : Tea.TeaModel {
    public var accessKeyId: String?

    public var accessKeySecret: String?

    public var securityToken: String?

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
        return map
    }

    public override func fromMap(_ dict: [String: Any]) -> Void {
        if dict.keys.contains("accessKeyId") {
            self.accessKeyId = dict["accessKeyId"] as! String
        }
        if dict.keys.contains("accessKeySecret") {
            self.accessKeySecret = dict["accessKeySecret"] as! String
        }
        if dict.keys.contains("securityToken") {
            self.securityToken = dict["securityToken"] as! String
        }
        if dict.keys.contains("protocol") {
            self.protocol_ = dict["protocol"] as! String
        }
        if dict.keys.contains("method") {
            self.method = dict["method"] as! String
        }
        if dict.keys.contains("regionId") {
            self.regionId = dict["regionId"] as! String
        }
        if dict.keys.contains("readTimeout") {
            self.readTimeout = dict["readTimeout"] as! Int
        }
        if dict.keys.contains("connectTimeout") {
            self.connectTimeout = dict["connectTimeout"] as! Int
        }
        if dict.keys.contains("httpProxy") {
            self.httpProxy = dict["httpProxy"] as! String
        }
        if dict.keys.contains("httpsProxy") {
            self.httpsProxy = dict["httpsProxy"] as! String
        }
        if dict.keys.contains("credential") {
            self.credential = dict["credential"] as! AlibabaCloudCredentials.Client
        }
        if dict.keys.contains("endpoint") {
            self.endpoint = dict["endpoint"] as! String
        }
        if dict.keys.contains("noProxy") {
            self.noProxy = dict["noProxy"] as! String
        }
        if dict.keys.contains("maxIdleConns") {
            self.maxIdleConns = dict["maxIdleConns"] as! Int
        }
        if dict.keys.contains("network") {
            self.network = dict["network"] as! String
        }
        if dict.keys.contains("userAgent") {
            self.userAgent = dict["userAgent"] as! String
        }
        if dict.keys.contains("suffix") {
            self.suffix = dict["suffix"] as! String
        }
        if dict.keys.contains("socks5Proxy") {
            self.socks5Proxy = dict["socks5Proxy"] as! String
        }
        if dict.keys.contains("socks5NetWork") {
            self.socks5NetWork = dict["socks5NetWork"] as! String
        }
        if dict.keys.contains("endpointType") {
            self.endpointType = dict["endpointType"] as! String
        }
        if dict.keys.contains("openPlatformEndpoint") {
            self.openPlatformEndpoint = dict["openPlatformEndpoint"] as! String
        }
        if dict.keys.contains("type") {
            self.type = dict["type"] as! String
        }
        if dict.keys.contains("signatureVersion") {
            self.signatureVersion = dict["signatureVersion"] as! String
        }
        if dict.keys.contains("signatureAlgorithm") {
            self.signatureAlgorithm = dict["signatureAlgorithm"] as! String
        }
        if dict.keys.contains("globalParameters") {
            var model = GlobalParameters()
            model.fromMap(dict["globalParameters"] as! [String: Any])
            self.globalParameters = model
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

    public override func fromMap(_ dict: [String: Any]) -> Void {
        if dict.keys.contains("headers") {
            self.headers = dict["headers"] as! [String: String]
        }
        if dict.keys.contains("query") {
            self.query = dict["query"] as! [String: String]
        }
        if dict.keys.contains("body") {
            self.body = dict["body"] as! Any
        }
        if dict.keys.contains("stream") {
            self.stream = dict["stream"] as! InputStream
        }
        if dict.keys.contains("hostMap") {
            self.hostMap = dict["hostMap"] as! [String: String]
        }
        if dict.keys.contains("endpointOverride") {
            self.endpointOverride = dict["endpointOverride"] as! String
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

    public override func fromMap(_ dict: [String: Any]) -> Void {
        if dict.keys.contains("action") {
            self.action = dict["action"] as! String
        }
        if dict.keys.contains("version") {
            self.version = dict["version"] as! String
        }
        if dict.keys.contains("protocol") {
            self.protocol_ = dict["protocol"] as! String
        }
        if dict.keys.contains("pathname") {
            self.pathname = dict["pathname"] as! String
        }
        if dict.keys.contains("method") {
            self.method = dict["method"] as! String
        }
        if dict.keys.contains("authType") {
            self.authType = dict["authType"] as! String
        }
        if dict.keys.contains("bodyType") {
            self.bodyType = dict["bodyType"] as! String
        }
        if dict.keys.contains("reqBodyType") {
            self.reqBodyType = dict["reqBodyType"] as! String
        }
        if dict.keys.contains("style") {
            self.style = dict["style"] as! String
        }
    }
}
