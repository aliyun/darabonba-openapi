English | [简体中文](README-CN.md)

![](https://aliyunsdk-pages.alicdn.com/icons/AlibabaCloud.svg)

## Alibaba Cloud Tea Util for Swift

## Requirements

- iOS 13.3+ / macOS 10.15+
- Xcode 11.3+
- Swift 5.6

## Installation

### CocoaPods

[CocoaPods](https://cocoapods.org) is a dependency manager for Cocoa projects. For usage and installation instructions, visit their website. To integrate `AlibabacloudOpenApi` into your Xcode project using CocoaPods, specify it in your `Podfile`:

```ruby
pod 'AlibabacloudOpenApi', '~> 1.0.0'
```

### Carthage

To integrate `AlibabacloudOpenApi` into your Xcode project using [Carthage](https://github.com/Carthage/Carthage), specify it in your `Cartfile`:

```ogdl
github "alibabacloud-sdk-swift/darabonba-openapi" "1.0.0"
```

### Swift Package Manager

To integrate `AlibabacloudOpenApi` into your Xcode project using [Swift Package Manager](https://swift.org/package-manager/) , adding `AlibabacloudOpenApi` to the `dependencies` value of your `Package.swift`.

```swift
dependencies: [
    .package(url: "https://github.com/alibabacloud-sdk-swift/darabonba-openapi.git", from: "1.0.0")
]
```

In addition, you also need to add `"AlibabacloudOpenApi"` to the `dependencies` of the `target`, as follows:

```swift
.target(
    name: "<your-project-name>",
    dependencies: [
        "AlibabacloudOpenApi",
    ])
```

## Issues

[Opening an Issue](https://github.com/alibabacloud-sdk-swift/darabonba-openapi/issues/new), Issues not conforming to the guidelines may be closed immediately.

## Changelog

Detailed changes for each release are documented in the [release notes](./ChangeLog.txt).

## References

- [Latest Release](https://github.com/alibabacloud-sdk-swift/darabonba-openapi/tree/master/swift)

## License

[Apache-2.0](http://www.apache.org/licenses/LICENSE-2.0)

Copyright (c) 2009-present, Alibaba Cloud All rights reserved.
