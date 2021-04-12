// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

using System;
using System.Text;

namespace Cmd2Serial
{
    public class Program
    {
        #region Main Statics

        public static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            Logger.LogWrite += Console.Write;

            var p = new Program(args);
            p.Run();
        }

        private static void PrintException(Exception ex)
        {
            var oldColor = StartConsoleError();

            if (!(ex is null))
            {
                Console.Error.WriteLine($"Error: { ex.Message }");

#if DEBUG
                Console.Error.WriteLine(ex.StackTrace);
#endif

                EndConsoleError(oldColor);

                if (null != ex.InnerException)
                {
                    PrintException(ex.InnerException);
                }
            }
        }

        private static ConsoleColor StartConsoleError()
        {
            ConsoleColor oldColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            return oldColor;
        }

        private static void EndConsoleError(ConsoleColor oldColor)
        {
            Console.ForegroundColor = oldColor;
        }

        #endregion

        #region Properties

        public readonly string[] Arguments;

        public ProgramArgs ProgramArgs { get; private set; }

        #endregion

        public Program(string[] args)
        {
            Arguments = args;
        }

        public void Run()
        {
            try
            {
                ProgramArgs = ProgramArgs.ParseArgs(Arguments);
                Logger.VerboseEnabled = ProgramArgs.Verbose;

                if (ProgramArgs.ShowVersion)
                {
                    ShowVersion();
                    return;
                }
                else if (ProgramArgs.ListPorts)
                {
                    ListPorts();
                    return;
                }
                else if (ProgramArgs.ShowHelp)
                {
                    ShowHelp();
                    return;
                }
                else if (string.IsNullOrWhiteSpace(ProgramArgs.Config.Command))
                {
                    throw new ParseArgumentsException("No command specified. See --help for details.");
                }

                var serialBridge = new SerialBridge(ProgramArgs.Config);

                Console.CancelKeyPress += (s, e) =>
                {
                    serialBridge.Cancel();
                    e.Cancel = true;
                };

                serialBridge.Start();
            }
            catch (ParseArgumentsException ex)
            {
                PrintException(ex);
                Console.WriteLine();
                ShowHelp();
            }
            catch (Exception ex)
            {
                PrintException(ex);
            }
        }

        #region Version

        private static void ShowVersion()
        {
            Console.WriteLine($"{ AppInfo.Name } v{ AppInfo.Version }");
            Console.WriteLine();
        }

        #endregion

        #region Help

        private static void ShowHelp()
        {
            Console.WriteLine("Usage: cmd2serial [--version] [--help]");
            Console.WriteLine("                  [options...] command [command args]");
            Console.WriteLine();

            Console.WriteLine("Options:");
            Console.WriteLine($"-h, --help         Show this help");
            Console.WriteLine($"-v, --verbose      Show verbose output");
            Console.WriteLine();

            Console.WriteLine($"-l, --list         List serial port names");
            Console.WriteLine();

            Console.WriteLine($"--PortName value   Set the port name (required)");
            Console.WriteLine($"--BaudRate value   Set the baud rate, default: {ProgramArgs.Default.Config.BaudRate}");
            Console.WriteLine($"--Parity value     Set the parity bit:");
            Console.WriteLine($"                     None - No parity check occurs (default)");
            Console.WriteLine($"                     Odd - Sets the parity bit so that the count of bits set is an odd number");
            Console.WriteLine($"                     Even - Sets the parity bit so that the count of bits set is an even number");
            Console.WriteLine($"                     Mark - Leaves the parity bit set to 1");
            Console.WriteLine($"                     Space - Leaves the parity bit set to 0");
            Console.WriteLine($"--DataBits value   Set the data bits, default: {ProgramArgs.Default.Config.DataBits}");
            Console.WriteLine($"--StopBits value   Set the stop bits:");
            Console.WriteLine($"                     One - One stop bit is used (default)");
            Console.WriteLine($"                     Two - Two stop bits are used");
            Console.WriteLine($"                     OnePointFive - 1.5 stop bits are used");
            Console.WriteLine($"--Handshake value  Set the flow control:");
            Console.WriteLine($"                     None - No control is used for the handshake (default)");
            Console.WriteLine($"                     XOnXOff - The XON/XOFF software control protocol is used");
            Console.WriteLine($"                     RequestToSend - Request-to-Send (RTS) hardware flow control is used");
            Console.WriteLine($"                     RequestToSendXOnXOff - Both RTS and XON/XOFF are used");
        }

        #endregion

        #region List Ports

        private static void ListPorts()
        {
            Console.WriteLine("Serial ports:");

            foreach (var portName in SerialBridge.PortNames)
            {
                Console.WriteLine(portName);
            }

            Console.WriteLine();
        }

        #endregion

    }
}
