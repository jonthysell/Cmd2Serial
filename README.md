# Cmd2Serial #

[![CI Build](https://github.com/jonthysell/Cmd2Serial/actions/workflows/ci.yml/badge.svg)](https://github.com/jonthysell/Cmd2Serial/actions/workflows/ci.yml)

Cmd2Serial is an application for forwarding the standard input and output from a console application to a serial port.

Cmd2Serial was written in C# and should run anywhere that supports [.NET 5.0](https://github.com/dotnet/core/blob/master/release-notes/5.0/5.0-supported-os.md). It has been officially tested on:

* Windows 10

## Installation ##

### Windows ###

The Windows release is a self-contained x86 binary and runs on Windows 7 SP1+, 8.1, and 10.

1. Download the latest Windows zip file (Cmd2Serial.Windows.zip) from https://github.com/jonthysell/Cmd2Serial/releases/latest
2. Extract the zip file

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
                  [options...] command [command args]

Options:
-h, --help         Show this help
-v, --verbose      Show verbose output

-l, --list         List serial port names

--PortName value   Set the port name (required)
--BaudRate value   Set the baud rate, default: 9600
--Parity value     Set the parity bit:
                     None - No parity check occurs (default)
                     Odd - Sets the parity bit so that the count of bits set is an odd number
                     Even - Sets the parity bit so that the count of bits set is an even number
                     Mark - Leaves the parity bit set to 1
                     Space - Leaves the parity bit set to 0
--DataBits value   Set the data bits, default: 8
--StopBits value   Set the stop bits:
                     One - One stop bit is used (default)
                     Two - Two stop bits are used
                     OnePointFive - 1.5 stop bits are used
--Handshake value  Set the flow control:
                     None - No control is used for the handshake (default)
                     XOnXOff - The XON/XOFF software control protocol is used
                     RequestToSend - Request-to-Send (RTS) hardware flow control is used
                     RequestToSendXOnXOff - Both RTS and XON/XOFF are used
```

## Errata ##

Cmd2Serial is open-source under the MIT license.

Cmd2Serial Copyright (c) 2021 Jon Thysell
