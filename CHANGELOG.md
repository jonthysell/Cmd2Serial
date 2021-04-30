# Cmd2Serial ChangeLog #

## next ##

* Added x64 Windows build
* Added --RedirectInput, --RedirectOutput, --RedirectError to control connections
* Changed --SerialEcho to take a boolean value

## v1.2 ##

* Fixed issue with disappearing cursor

## v1.1 ##

* Refactored bridge to remove encoding assumptions and enable more scenarios
* Cleaned up help and usage
* Added --SerialToCommandNewLines and --CommandToSerialNewLines flags to optionally convert newlines in either direction
* Added --SerialEcho flag to optionally echo serial input back to the serial output

## v1.0 ##

* First release
