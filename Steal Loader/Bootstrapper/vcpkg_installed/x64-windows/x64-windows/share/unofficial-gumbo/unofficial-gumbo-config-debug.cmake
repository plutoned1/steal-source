#----------------------------------------------------------------
# Generated CMake target import file for configuration "Debug".
#----------------------------------------------------------------

# Commands may need to know the format version.
set(CMAKE_IMPORT_FILE_VERSION 1)

# Import target "unofficial::gumbo::gumbo" for configuration "Debug"
set_property(TARGET unofficial::gumbo::gumbo APPEND PROPERTY IMPORTED_CONFIGURATIONS DEBUG)
set_target_properties(unofficial::gumbo::gumbo PROPERTIES
  IMPORTED_LINK_INTERFACE_LANGUAGES_DEBUG "C"
  IMPORTED_LOCATION_DEBUG "${_IMPORT_PREFIX}/debug/lib/gumbo.lib"
  )

list(APPEND _cmake_import_check_targets unofficial::gumbo::gumbo )
list(APPEND _cmake_import_check_files_for_unofficial::gumbo::gumbo "${_IMPORT_PREFIX}/debug/lib/gumbo.lib" )

# Commands beyond this point should not need to know the version.
set(CMAKE_IMPORT_FILE_VERSION)
