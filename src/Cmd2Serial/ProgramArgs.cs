// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

using System;
using System.IO.Ports;

namespace Cmd2Serial
{
    public class ProgramArgs
    {
        public static readonly ProgramArgs Default = new ProgramArgs();

        public bool Verbose { get; private set; } = false;

        public bool ShowVersion { get; private set; } = false;

        public bool ShowHelp { get; private set; } = false;

        public bool ListPorts { get; private set; } = false;

        public SerialBridgeConfig Config { get; private set; } = new SerialBridgeConfig();

        private ProgramArgs() { }

        public static ProgramArgs ParseArgs(string[] args)
        {
            var result = new ProgramArgs();

            if (null != args && args.Length > 0)
            {
                bool parseCommand = false;
                for (int i = 0; i < args.Length; i++)
                {
                    if (parseCommand)
                    {
                        if (string.IsNullOrEmpty(result.Config.Command))
                        {
                            result.Config.Command = args[i];
                        }
                        else
                        {
                            result.Config.CommandArgs += args[i] + " ";
                        }
                    }
                    else
                    {
                        string arg = args[i];
                        try
                        {
                            switch (arg.ToLower())
                            {
                                case "--version":
                                case "/version":
                                    result.ShowVersion = true;
                                    break;
                                case "-?":
                                case "/?":
                                case "-h":
                                case "/h":
                                case "--help":
                                case "/help":
                                    result.ShowHelp = true;
                                    break;
                                case "-v":
                                case "/v":
                                case "--verbose":
                                case "/verbose":
                                    result.Verbose = true;
                                    break;
                                case "-l":
                                case "/l":
                                case "--list":
                                case "/list":
                                    result.ListPorts = true;
                                    break;
                                case "--portname":
                                case "/portname":
                                    if (!SerialBridge.TryParsePortName(args[++i], out string portName))
                                    {
                                        throw new Exception($"Port name invalid. Try using --list to find valid serial ports.");
                                    }
                                    result.Config.PortName = portName;
                                    break;
                                case "--baudrate":
                                case "/baudrate":
                                    if (!int.TryParse(args[++i], out int baudRate) || baudRate <= 0)
                                    {
                                        throw new Exception("Baud rate value invalid. Must be an integer > 0.");
                                    }
                                    result.Config.BaudRate = baudRate;
                                    break;
                                case "--parity":
                                case "/parity":
                                    if (!Enum.TryParse(args[++i], true, out Parity parity))
                                    {
                                        throw new Exception($"Parity bit value invalid. See --help for valid values.");
                                    }
                                    result.Config.Parity = parity;
                                    break;
                                case "--databits":
                                case "/databits":
                                    if (!int.TryParse(args[++i], out int dataBits) || dataBits <= 0)
                                    {
                                        throw new Exception("Data bits value invalid. Must be an integer > 0.");
                                    }
                                    result.Config.DataBits = dataBits;
                                    break;
                                case "--stopbits":
                                case "/stopbits":
                                    if (!Enum.TryParse(args[++i], true, out StopBits stopBits) || stopBits == StopBits.None)
                                    {
                                        throw new Exception($"Stop bits value invalid. See --help for valid values.");
                                    }
                                    result.Config.StopBits = stopBits;
                                    break;
                                case "--handshake":
                                case "/handshake":
                                    if (!Enum.TryParse(args[++i], true, out Handshake handshake))
                                    {
                                        throw new Exception($"Handshake value invalid. See --help for valid values.");
                                    }
                                    result.Config.Handshake = handshake;
                                    break;
                                default:
                                    parseCommand = true;
                                    i--;
                                    break;
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new ParseArgumentsException($"Unable to parse \"{args[i]}\" for {arg}.", ex);
                        }
                    }
                }
            }

            return result;
        }
    }

    #region Exceptions

    public class ParseArgumentsException : Exception
    {
        public ParseArgumentsException(string message) : base(message) { }

        public ParseArgumentsException(string message, Exception innerException) : base(message, innerException) { }
    }

    #endregion
}
