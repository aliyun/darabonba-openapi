// swift-tools-version:5.6
// The swift-tools-version declares the minimum version of Swift required to build this package.
import PackageDescription

let package = Package(
        name: "AlibabacloudOpenApi",
        platforms: [.macOS(.v10_15),
                    .iOS(.v13),
                    .tvOS(.v13),
                    .watchOS(.v6)],
        products: [
            .library(
                    name: "AlibabacloudOpenApi",
                    targets: ["AlibabacloudOpenApi"])
        ],
        dependencies: [
            // Dependencies declare other packages that this package depends on.
            .package(url: "https://github.com/aliyun/tea-swift.git", from: "1.0.1"),
            .package(url: "https://github.com/alibabacloud-sdk-swift/tea-utils", from: "1.0.1"),
            .package(url: "https://github.com/aliyun/credentials-swift", from: "1.0.0"),
            .package(url: "https://github.com/alibabacloud-sdk-swift/openapi-util", from: "1.0.1"),
        ],
        targets: [
            .target(
                    name: "AlibabacloudOpenApi",
                    dependencies: [
                        .product(name: "Tea", package: "tea-swift"),
                        .product(name: "TeaUtils", package: "tea-utils"),
                        .product(name: "AlibabaCloudCredentials", package: "credentials-swift"),
                        .product(name: "AlibabaCloudOpenApiUtil", package: "openapi-util"),
                    ]),
        ],
        swiftLanguageVersions: [.v5]
)
