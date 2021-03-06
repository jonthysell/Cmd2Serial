# Cmd2Serial #

[![CI Build](https://github.com/jonthysell/Cmd2Serial/actions/workflows/ci.yml/badge.svg)](https://github.com/jonthysell/Cmd2Serial/actions/workflows/ci.yml)

Cmd2Serial is an application for forwarding the standard input and output from a console application to a serial port.

Cmd2Serial was written in C# and should run anywhere that supports [.NET 5.0](https://github.com/dotnet/core/blob/master/release-notes/5.0/5.0-supported-os.md). It has been officially tested on:

* Windows 10

## Installation ##

### Windows ###

The Windows release is a self-contained x86/x64 binary which run on Windows 7 SP1+, 8.1, and 10.

1. Download the latest Windows zip file (Cmd2Serial.Win64.zip *or* Cmd2Serial.Win32.zip) from https://github.com/jonthysell/Cmd2Serial/releases/latest
2. Extract the zip file

**Note:** If you're unsure which version to download, try Cmd2Serial.Win64.zip first. Most modern PCs are 64-Bit.

### MacOS ###

The MacOS release is a self-contained x64 binary and runs on OSX >= 10.13.

1. Download the latest MacOS tar.gz file (Cmd2Serial.MacOS.tar.gz) from https://github.com/jonthysell/Cmd2Serial/releases/latest
2. Extract the tar.gz file

### Linux ###

The Linux release is a self-contained x64 binary and runs on many Linux distributions.

1. Download the latest Linux tar.gz file (Cmd2Serial.Linux.tar.gz) from https://github.com/jonthysell/Cmd2Serial/releases/latest
2. Extract the tar.gz file

## Usage ##

```none
Usage: cmd2serial [--version] [--help]
                  [options...] command [args...]

Options:
-h, --help        Show this help
-v, --verbose     Show verbose output

-l, --list        List serial port names

--PortName value  The port name to use (required)

--BaudRate value  The baud rate to use, default: 9600

--Parity value  The parity bit to use:
  None - No parity check occurs (default)
  Odd - Sets the parity bit so that the count of bits set is an odd number
  Even - Sets the parity bit so that the count of bits set is an even number
  Mark - Leaves the parity bit set to 1
  Space - Leaves the parity bit set to 0

--DataBits value  The number of data bits to use, default: 8

--StopBits value  The stop bits to use:
  One          - One stop bit is used (default)
  Two          - Two stop bits are used
  OnePointFive - 1.5 stop bits are used

--Handshake value  The flow control to use:
  None - No control is used for the handshake (default)
  XOnXOff - The XON/XOFF software control protocol is used
  RequestToSend - Request-to-Send (RTS) hardware flow control is used
  RequestToSendXOnXOff - Both RTS and XON/XOFF are used

--SerialToCommandNewLines value  Convert new lines from serial output to command input:
  None - No conversion (default)
  CR   - Convert new lines to CR
  LF   - Convert new lines to LF
  CRLF - Convert new lines to CRLF

--CommandToSerialNewLines value  Convert new lines from command output to serial input:
  None - No conversion (default)
  CR   - Convert new lines to CR
  LF   - Convert new lines to LF
  CRLF - Convert new lines to CRLF

--SerialEcho value  Echo input from the serial port back out to the serial port, default: False

--RedirectOutput value  Pass command output to the serial input, default: True

--RedirectError value  Pass command errors to the serial input, default: True

--RedirectInput value  Pass serial output to the command input, default: True
```

### Examples ###

Forwarding a Windows command prompt to the COM3 serial port: `cmd2serial --PortName COM3 -SerialToCommandNewLines CRLF --SerialEcho True cmd.exe /A /Q`

## Errata ##

Cmd2Serial is open-source under the MIT license.

Cmd2Serial Copyright (c) 2021 Jon Thysell
