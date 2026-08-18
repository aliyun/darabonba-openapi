import Foundation
import Tea

open class AlibabaCloudException : Tea.ReuqestError {
    public override init(_ map: [String: Any]?) {
        super.init(map)
        self.name = "AlibabaCloudException"
    }
}

open class ClientException : AlibabaCloudException {
    public override init(_ map: [String: Any]?) {
        super.init(map)
        self.name = "ClientException"
    }
}

open class ServerException : AlibabaCloudException {
    public override init(_ map: [String: Any]?) {
        super.init(map)
        self.name = "ServerException"
    }
}

open class ThrottlingException : AlibabaCloudException {
    public override init(_ map: [String: Any]?) {
        super.init(map)
        self.name = "ThrottlingException"
    }
}
