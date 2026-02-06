find_package(alibabacloud_credentials QUIET)

if(alibabacloud_credentials_FOUND)
  message(STATUS "Found alibabacloud_credentials version ${alibabacloud_credentials_VERSION}")
else()
  message(STATUS "alibabacloud_credentials could not be located, Building alibabacloud_credentials instead.")
  include(FetchContent)

  FetchContent_Declare(
    _alibabacloud_credentials
    GIT_REPOSITORY https://github.com/aliyun/credentials-cpp.git
    GIT_TAG master
    CMAKE_ARGS
      -DENABLE_UNIT_TESTS:BOOL="0")

  FetchContent_MakeAvailable(_alibabacloud_credentials)
  message(STATUS "Added alibabacloud_credentials source : ${_alibabacloud_credentials_SOURCE_DIR}")
  message(STATUS "Added alibabacloud_credentials binary : ${_alibabacloud_credentials_BINARY_DIR}")
endif()