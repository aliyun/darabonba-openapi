Pod::Spec.new do |spec|

  spec.name         = "AlibabacloudOpenApi"
  spec.version      = "1.0.2"
  spec.license      = "Apache 2.0"
  spec.summary      = "Alibaba Cloud openapi SDK Library for Swift(5.6)"
  spec.homepage     = "https://github.com/alibabacloud-sdk-swift/darabonba-openapi" 
  spec.author       = { "Alibaba Cloud SDK" => "sdk-team@alibabacloud.com" }

  spec.source       = { :git => spec.homepage + '.git', :tag => spec.version }
  spec.source_files = 'Sources/**/*.swift'

  spec.ios.framework   = 'Foundation'

  spec.ios.deployment_target     = '13.0'
  spec.osx.deployment_target     = '10.15'
  spec.watchos.deployment_target = '6.0'
  spec.tvos.deployment_target    = '13.0'

  spec.dependency 'Tea',  '~> 1.0.1'
  spec.dependency 'TeaUtils',  '~> 1.0.2'
  spec.dependency 'AlibabaCloudCredentials',  '~> 1.0.1'
  spec.dependency 'AlibabaCloudOpenApiUtil',  '~> 1.0.1'

  spec.swift_version='5.6'
end
