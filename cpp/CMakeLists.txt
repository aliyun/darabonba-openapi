
cmake_minimum_required(VERSION 3.5 FATAL_ERROR)

project(alibabacloud_open_api_v2 VERSION "1.0.0")

# <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<< General set up >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> #
set(CMAKE_CXX_STANDARD 11)
set(CMAKE_CXX_EXTENSIONS OFF)
set(CMAKE_CXX_STANDARD_REQUIRED ON)
set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -std=c++11")
set(CMAKE_INSTALL_PREFIX "${PROJECT_SOURCE_DIR}/${PROJECT_NAME}_build")
SET(CMAKE_FIND_LIBRARY_SUFFIXES "${CMAKE_FIND_LIBRARY_SUFFIXES}" "_d.dylib")
message("C++ compiler flags: ${CMAKE_CXX_FLAGS}")

if (CMAKE_CXX_COMPILER_LOADED)
    message(STATUS "The C++ compiler ID is: ${CMAKE_CXX_COMPILER_ID}")
    message(STATUS "The C++ compiler version is: ${CMAKE_CXX_COMPILER_VERSION}")
endif ()

if (NOT CMAKE_BUILD_TYPE)
    set(CMAKE_BUILD_TYPE Release CACHE STRING "Build type" FORCE)
endif ()
message(STATUS "Build type: ${CMAKE_BUILD_TYPE}")

# <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<< Target set up >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> #=
include_directories(${CMAKE_SOURCE_DIR}/include)

set(SOURCE_FILES
        src/Client.cpp
        src/Utils.cpp
        src/POP.cpp)

add_library(${PROJECT_NAME} SHARED ${SOURCE_FILES})
        
target_include_directories(${PROJECT_NAME}
        PRIVATE
        ${CMAKE_CURRENT_SOURCE_DIR}/include)

set_target_properties(${PROJECT_NAME}
        PROPERTIES
        POSITION_INDEPENDENT_CODE 1
        SOVERSION ${PROJECT_VERSION_MAJOR}
        OUTPUT_NAME "${PROJECT_NAME}"
        DEBUG_POSTFIX "_d"
        PUBLIC_HEADER "${headers}"
        MACOSX_RPATH ON
        WINDOWS_EXPORT_ALL_SYMBOLS ON)

# <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<< External set up >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> #
add_subdirectory(external)
find_package(OpenSSL REQUIRED)
find_library(CURL_LIB curl)
if(CMAKE_HOST_WIN32)
  ExternalProject_Get_Property(curl INSTALL_DIR)
    set(curl_install_dir ${INSTALL_DIR})
    add_dependencies(${PROJECT_NAME}
            curl)
    target_include_directories(${PROJECT_NAME}
            PRIVATE	${curl_install_dir}/include)
    target_link_libraries(${PROJECT_NAME}
            OpenSSL::SSL OpenSSL::Crypto
            Crypt32
            Rpcrt4
            Ws2_32
            debug ${curl_install_dir}/lib/libcurl-d.lib
            optimized ${curl_install_dir}/lib/libcurl.lib
            darabonba_core
            alibabacloud_credential
            
            )
    target_compile_definitions(${PROJECT_NAME}
            PRIVATE CURL_STATICLIB
            _CRT_SECURE_NO_WARNINGS)
elseif(APPLE)
  find_library(CFLIB CoreFoundation)
  find_library(UTIL_LIB alibabacloud_open_api_v2 ${CMAKE_CURRENT_SOURCE_DIR}/deps/lib)
  target_link_libraries(${PROJECT_NAME} 
      OpenSSL::SSL OpenSSL::Crypto
      ${CFLIB}
      ${CURL_LIB} 
      darabonba_core
      alibabacloud_credential
            )
  target_compile_definitions(${PROJECT_NAME} PRIVATE GUID_CFUUID)
else()
  target_link_libraries(${PROJECT_NAME} 
      OpenSSL::SSL OpenSSL::Crypto
      ${CURL_LIB} 
      darabonba_core
      alibabacloud_credential
            )
endif()


# <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<< Install set up >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> #
message(STATUS "${PROJECT_NAME} : Project will be installed to ${CMAKE_INSTALL_PREFIX}")

include(GNUInstallDirs)

set(CMAKE_ARCHIVE_OUTPUT_DIRECTORY ${PROJECT_BINARY_DIR}/${CMAKE_INSTALL_LIBDIR})
set(CMAKE_LIBRARY_OUTPUT_DIRECTORY ${PROJECT_BINARY_DIR}/${CMAKE_INSTALL_LIBDIR})
set(CMAKE_RUNTIME_OUTPUT_DIRECTORY ${PROJECT_BINARY_DIR}/${CMAKE_INSTALL_BINDIR})

set(INSTALL_LIBDIR ${CMAKE_INSTALL_LIBDIR} CACHE PATH "Installation directory for libraries")
set(INSTALL_BINDIR ${CMAKE_INSTALL_BINDIR} CACHE PATH "Installation directory for executables")
set(INSTALL_INCLUDEDIR ${CMAKE_INSTALL_INCLUDEDIR} CACHE PATH "Installation directory for header files")
if(WIN32 AND NOT CYGWIN)
    set(DEF_INSTALL_CMAKEDIR CMake)
else()
    set(DEF_INSTALL_CMAKEDIR share/cmake/${PROJECT_NAME})
endif()
set(INSTALL_CMAKEDIR ${DEF_INSTALL_CMAKEDIR} CACHE PATH "Installation directory for CMake files")

foreach(p LIB BIN INCLUDE CMAKE)
    file(TO_NATIVE_PATH ${CMAKE_INSTALL_PREFIX}/${INSTALL_${p}DIR} _path )
    message(STATUS "Installing ${p} components to ${_path}")
    unset(_path)
endforeach()

# Prepare RPATH
file(RELATIVE_PATH _rel ${CMAKE_INSTALL_PREFIX}/${INSTALL_BINDIR} ${CMAKE_INSTALL_PREFIX})
if(APPLE)
    set(_rpath "@loader_path/${_rel}")
else()
    set(_rpath "\$ORIGIN/${_rel}")
endif()
file(TO_NATIVE_PATH "${_rpath}/${INSTALL_LIBDIR}" install_RPATH)

set_target_properties(${PROJECT_NAME}
        PROPERTIES
        MACOSX_RPATH ON
        SKIP_BUILD_RPATH OFF
        BUILD_WITH_INSTALL_RPATH OFF
        INSTALL_RPATH "${install_RPATH}"
        INSTALL_RPATH_USE_LINK_PATH ON)

install(
        TARGETS
        ${PROJECT_NAME}
        EXPORT
        alibabacloud_open_api_v2Targets
        ARCHIVE
        DESTINATION ${INSTALL_LIBDIR}
        COMPONENT lib
        RUNTIME
        DESTINATION ${INSTALL_BINDIR}
        COMPONENT bin
        LIBRARY
        DESTINATION ${INSTALL_LIBDIR}
        COMPONENT lib
        PUBLIC_HEADER
        DESTINATION ${INSTALL_INCLUDEDIR}/alibabacloud
        COMPONENT dev
)

# Install cmake config
install(
        EXPORT
        alibabacloud_open_api_v2Targets
        NAMESPACE
        "alibabacloud_open_api_v2::"
        DESTINATION
        ${INSTALL_CMAKEDIR}
        COMPONENT
        dev
)
include(CMakePackageConfigHelpers)

write_basic_package_version_file(
        ${CMAKE_CURRENT_BINARY_DIR}/${PROJECT_NAME}ConfigVersion.cmake
        VERSION ${PROJECT_VERSION}
        COMPATIBILITY SameMajorVersion
)
configure_package_config_file(
        ${PROJECT_SOURCE_DIR}/cmake/${PROJECT_NAME}Config.cmake.in
        ${CMAKE_CURRENT_BINARY_DIR}/${PROJECT_NAME}Config.cmake
        INSTALL_DESTINATION ${INSTALL_CMAKEDIR}
)
install(
        FILES
        ${CMAKE_CURRENT_BINARY_DIR}/${PROJECT_NAME}Config.cmake
        ${CMAKE_CURRENT_BINARY_DIR}/${PROJECT_NAME}ConfigVersion.cmake
        DESTINATION
        ${INSTALL_CMAKEDIR}
)

# <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<< Test set up >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> #
option(BUILD_TESTS "Build test programs" ON)

if(BUILD_TESTS)
    enable_testing()
    
    # Try to find GoogleTest locally first
    find_package(GTest QUIET)
    
    if(NOT GTest_FOUND)
        # Fetch GoogleTest if not found
        include(FetchContent)
        FetchContent_Declare(
            googletest
            GIT_REPOSITORY https://github.com/google/googletest.git
            GIT_TAG v1.14.0
        )
        # For Windows: Prevent overriding the parent project's compiler/linker settings
        set(gtest_force_shared_crt ON CACHE BOOL "" FORCE)
        FetchContent_MakeAvailable(googletest)
    endif()
    
    # Create test executable
    add_executable(test_utils tests/test_utils.cpp)
    
    # Add dependencies first
    add_dependencies(test_utils ${PROJECT_NAME} darabonba_core alibabacloud_credential)
    
    target_include_directories(test_utils PRIVATE
        ${CMAKE_CURRENT_SOURCE_DIR}/include
        ${CMAKE_CURRENT_BINARY_DIR}/_deps/_darabonba_core-src/include
        ${CMAKE_CURRENT_BINARY_DIR}/_deps/_alibabacloud_credential-src/include
    )
    
    if(GTest_FOUND)
        target_link_libraries(test_utils
            ${PROJECT_NAME}
            darabonba_core
            alibabacloud_credential
            GTest::gtest
            GTest::gtest_main
        )
    else()
        target_link_libraries(test_utils
            ${PROJECT_NAME}
            darabonba_core
            alibabacloud_credential
            gtest
            gtest_main
        )
    endif()
    
    # Add test to CTest
    add_test(NAME UtilsTest COMMAND test_utils)
endif()