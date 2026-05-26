message(STATUS "Install libraries")

# Pre-create config-specific directories for Windows MSVC to avoid post-build copy errors
if(MSVC)
  foreach(config ${CMAKE_CONFIGURATION_TYPES})
    file(MAKE_DIRECTORY "${CMAKE_CURRENT_BINARY_DIR}/_alibabacloud_credentials-build/src/${config}")
    file(MAKE_DIRECTORY "${CMAKE_CURRENT_BINARY_DIR}/_darabonba_core-build/src/${config}")
  endforeach()
endif()

add_subdirectory(alibabacloud_credentials)
add_subdirectory(darabonba_core)
