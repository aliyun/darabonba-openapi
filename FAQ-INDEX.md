# Alibaba Cloud OpenAPI SDK - FAQ Index

This repository contains Frequently Asked Questions (FAQ) documentation for all language implementations of the Alibaba Cloud OpenAPI SDK.

## Language-Specific FAQs

Each language has its own FAQ file with common issues, best practices, and solutions:

### [Python FAQ](./python/FAQ.md)
- Dependency version mismatches
- Chinese locale date format issues
- Timeout configuration
- Async retry patterns
- [View Python FAQ →](./python/FAQ.md)

### [TypeScript/JavaScript FAQ](./ts/FAQ.md)
- keepAlive configuration
- Credential management
- Proxy settings
- Type definitions and IntelliSense
- [View TypeScript FAQ →](./ts/FAQ.md)

### [Golang FAQ](./golang/FAQ.md)
- TLS certificate issues
- Context support
- Dependency management
- Swagger documentation generation
- [View Golang FAQ →](./golang/FAQ.md)

### [PHP FAQ](./php/FAQ.md)
- Class not found errors
- Composer autoload issues
- Argument passing by reference
- Signature algorithm configuration
- [View PHP FAQ →](./php/FAQ.md)

### [C# FAQ](./csharp/FAQ.md)
- NameInMap serialization
- Tea package location
- NuGet configuration
- Dependency injection patterns
- [View C# FAQ →](./csharp/FAQ.md)

### [Java FAQ](./java/FAQ.md)
- Maven/Gradle dependency conflicts
- Character encoding issues
- SSL certificate verification
- Connection timeout configuration
- [View Java FAQ →](./java/FAQ.md)

### [C++ FAQ](./cpp/FAQ.md)
- Compilation errors
- CMake configuration
- Memory management
- Thread safety
- [View C++ FAQ →](./cpp/FAQ.md)

### [Swift FAQ](./swift/FAQ.md)
- SPM and CocoaPods setup
- Async/await patterns
- Memory management
- SwiftUI integration
- [View Swift FAQ →](./swift/FAQ.md)

---

## Common Topics Across All Languages

### Authentication & Credentials
All SDKs support multiple authentication methods:
- Access Key (AccessKeyId + AccessKeySecret)
- STS Token (Temporary credentials)
- RAM Role
- Bearer Token
- Credential Chain (environment variables, config files, instance metadata)

### Error Handling
All SDKs implement a consistent exception hierarchy:
- **ClientException**: Client-side errors (invalid parameters, configuration issues)
- **ServerException**: Server-side errors (5xx status codes)
- **ThrottlingException**: Rate limiting errors with retry-after information
- **AlibabaCloudException**: Base exception class

### Configuration Best Practices
- Store credentials in environment variables or secure vaults
- Set appropriate timeout values based on operation type
- Enable retry with exponential backoff for transient failures
- Configure connection pooling for better performance
- Use proxy settings when behind corporate firewalls

### Debugging Tips
- Enable SDK debug logging
- Check network connectivity and proxy settings
- Verify credential permissions
- Review request/response details
- Use appropriate tools (debuggers, profilers, network analyzers)

---

## Related Issues

The FAQs are based on analysis of GitHub issues in this repository. Key issues addressed include:

- #238 - TypeScript keepAlive configuration
- #225 - Golang missing license file
- #221 - Golang Swagger comment parsing
- #218 - Python wheel distribution
- #215 - TypeScript signature in private networks
- #206 - PHP argument reference passing
- #203 - Python dependency version mismatch
- #177 - Golang undefined types
- #172 - TypeScript constructor issues
- #171 - Golang context support
- #169 - C# NameInMap serialization
- #162, #156 - TypeScript getCredential errors
- #159 - TypeScript type hints
- #155, #89 - SDK usage documentation
- #140 - Python timeout errors
- #136 - Python async retry
- #134 - TypeScript proxy settings
- #133 - Python package version metadata
- #132, #130 - PHP class not found
- #131 - Golang TLS certificate errors
- #125 - Package version availability
- #115 - C# Tea package location
- #113 - PHP signature algorithm
- #112 - PHP tea-xml dependency
- #99 - Python locale date format
- #98 - Golang SDK selection

[View all issues →](https://github.com/aliyun/darabonba-openapi/issues)

---

## Contributing

If you encounter issues not covered in these FAQs, please:

1. Check the [GitHub Issues](https://github.com/aliyun/darabonba-openapi/issues) for similar problems
2. Review the language-specific FAQ for your SDK
3. If your issue is new, [open an issue](https://github.com/aliyun/darabonba-openapi/issues/new) with:
   - SDK language and version
   - Detailed error message
   - Code snippet to reproduce
   - Expected vs actual behavior

Your contributions help improve the SDK for everyone!

---

## Additional Resources

### Official Documentation
- [Alibaba Cloud API Documentation](https://help.aliyun.com/document_detail/README.html)
- [OpenAPI SDK Homepage](https://github.com/aliyun/darabonba-openapi)

### SDK Source Code
- [Python SDK](./python/)
- [TypeScript SDK](./ts/)
- [Golang SDK](./golang/)
- [PHP SDK](./php/)
- [C# SDK](./csharp/)
- [Java SDK](./java/)
- [C++ SDK](./cpp/)
- [Swift SDK](./swift/)

### Package Repositories
- Python: [PyPI](https://pypi.org/project/alibabacloud-tea-openapi/)
- TypeScript: [npm](https://www.npmjs.com/package/@alicloud/openapi-client)
- Golang: [Go Packages](https://pkg.go.dev/github.com/alibabacloud-go/darabonba-openapi/v2/client)
- PHP: [Packagist](https://packagist.org/packages/alibabacloud/darabonba-openapi)
- C#: [NuGet](https://www.nuget.org/packages/AlibabaCloud.OpenApiClient/)
- Java: [Maven Central](https://search.maven.org/search?q=g:com.aliyun%20AND%20a:tea-openapi)

---

## License

[Apache-2.0](http://www.apache.org/licenses/LICENSE-2.0)

Copyright (c) 2009-present, Alibaba Cloud All rights reserved.
