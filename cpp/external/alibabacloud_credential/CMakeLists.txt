find_package(alibabacloud_credential QUIET)

if(alibabacloud_credential_FOUND)
  message(STATUS "Found alibabacloud_credential version ${alibabacloud_credential_VERSION}")
else()
  message(STATUS "alibabacloud_credential could not be located, Building alibabacloud_credential instead.")
  include(FetchContent)

  FetchContent_Declare(
    _alibabacloud_credential
    GIT_REPOSITORY https://github.com/aliyun/credentials-cpp.git
    GIT_TAG master
    CMAKE_ARGS
      -DENABLE_UNIT_TESTS:BOOL="0")

  FetchContent_MakeAvailable(_alibabacloud_credential)
  message(STATUS "Added alibabacloud_credential source : ${_alibabacloud_credential_SOURCE_DIR}")
  message(STATUS "Added alibabacloud_credential binary : ${_alibabacloud_credential_BINARY_DIR}")

  target_include_directories(${PROJECT_NAME} PRIVATE "${_alibabacloud_credential_SOURCE_DIR}/include")
endif()