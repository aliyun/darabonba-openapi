find_package(alibabacloud_open_api_util QUIET)

if(alibabacloud_open_api_util_FOUND)
  message(STATUS "Found alibabacloud_open_api_util version ${alibabacloud_open_api_util_VERSION}")
else()
  message(STATUS "alibabacloud_open_api_util could not be located, Building alibabacloud_open_api_util instead.")
  include(FetchContent)

  FetchContent_Declare(
    _alibabacloud_open_api_util
    GIT_REPOSITORY https://github.com/alibabacloud-sdk-cpp/openapi-util.git
    GIT_TAG master
    CMAKE_ARGS
      -DENABLE_UNIT_TESTS:BOOL="0")

  FetchContent_GetProperties(_alibabacloud_open_api_util)
  if(NOT _alibabacloud_open_api_util_POPULATED)
    FetchContent_Populate(_alibabacloud_open_api_util)
    message(STATUS "Added alibabacloud_open_api_util source : ${_alibabacloud_open_api_util_SOURCE_DIR}")
    message(STATUS "Added alibabacloud_open_api_util binary : ${_alibabacloud_open_api_util_BINARY_DIR}")
    add_subdirectory(
      ${_alibabacloud_open_api_util_SOURCE_DIR}
      ${_alibabacloud_open_api_util_BINARY_DIR}
    )
  endif()
  target_include_directories(alibabacloud_open_api PRIVATE "${_alibabacloud_open_api_util_SOURCE_DIR}/include")
  if(ENABLE_UNIT_TESTS)
    target_include_directories(tests_alibabacloud_open_api PRIVATE "${_alibabacloud_open_api_util_SOURCE_DIR}/include")
  endif()
endif()
