// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

using System;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.IO.Ports;

namespace Cmd2Serial
{
    public class SerialBridge
    {
        public SerialBridgeConfig Config { get; private set; }

        private readonly SerialPort _sp;

        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        private readonly Process _process = new Process();

        public static string[] PortNames => _portNames ??= SerialPort.GetPortNames();
        private static string[] _portNames;

        public static string DefaultPortName => PortNames is not null && PortNames.Length > 0 ? PortNames[0] : "";

        public static bool TryParsePortName(string s, out string result)
        {
            foreach (var portName in PortNames)
            {
                if (portName.Equals(s?.Trim(), StringComparison.InvariantCultureIgnoreCase))
                {
                    result = portName;
                    return true;
                }
            }

            result = "";
            return false;
        }

        public SerialBridge(SerialBridgeConfig config)
        {
            Config = config;
            _sp = new SerialPort(Config.PortName, Config.BaudRate, Config.Parity, Config.DataBits, Config.StopBits)
            {
                Encoding = Encoding.ASCII,
                Handshake = config.Handshake,
                NewLine = Environment.NewLine,
            };
        }

        public void Start()
        {
            var task = StartAsync();
            task.Wait();
        }

        public async Task StartAsync()
        {
            Logger.VerboseWrite($"Opening serial port...");
            _sp.Open();
            Logger.VerboseWriteLine($" done.");

            var token = _cts.Token;

            _process.StartInfo.FileName = Config.Command;
            _process.StartInfo.Arguments = Config.CommandArgs;
            _process.StartInfo.UseShellExecute = false;
            _process.StartInfo.CreateNoWindow = true;
            _process.StartInfo.RedirectStandardInput = true;
            _process.StartInfo.RedirectStandardOutput = true;
            _process.StartInfo.RedirectStandardError = true;
            _process.StartInfo.StandardOutputEncoding = Encoding.ASCII;

            _process.EnableRaisingEvents = true;
            _process.Exited += Process_Exited;

            Logger.VerboseWrite($"Starting command process...");
            try
            {
                _process.Start();
            }
            catch (Exception ex)
            {
                throw new Exception($"Unable to start command process \"{Config.FullCommand}\".", ex);
            }
            
            Logger.VerboseWriteLine($" done.");

            Logger.VerboseWrite($"Linking command output to serial...");
            var cmdReader = _process.StandardOutput;
            var fromCmdToSerialTask = Task.Run(async () =>
            {
                char[] buffer = new char[2048];
                while (!token.IsCancellationRequested)
                {
                    if (!cmdReader.EndOfStream)
                    {
                        int numChars = await cmdReader.ReadAsync(buffer, 0, buffer.Length);
                        if (numChars > 0)
                        {
                            _sp.Write(buffer, 0, numChars);
                            Console.Write(buffer, 0, numChars);
                        }
                    }

                    await Task.Yield();
                }
            });
            Logger.VerboseWriteLine($" done.");

            Logger.VerboseWrite($"Linking command errors to serial...");
            var cmdErrorReader = _process.StandardError;
            var fromCmdErrorToSerialTask = Task.Run(async () =>
            {
                char[] buffer = new char[2048];
                while (!token.IsCancellationRequested)
                {
                    if (!cmdErrorReader.EndOfStream)
                    {
                        int numChars = await cmdErrorReader.ReadAsync(buffer, 0, buffer.Length);
                        if (numChars > 0)
                        {
                            _sp.Write(buffer, 0, numChars);
                            Console.Write(buffer, 0, numChars);
                        }
                    }

                    await Task.Yield();
                }
            });
            Logger.VerboseWriteLine($" done.");

            Logger.VerboseWrite($"Linking command input to serial...");
            var cmdWriter = _process.StandardInput;
            var fromSerialToCmdTask = Task.Run(async () =>
            {
                char previousValue = '\0';
                while (!token.IsCancellationRequested)
                {
                    var c = _sp.ReadChar();
                    var newValue = (char)c;

                    // Echo
                    _sp.Write(newValue.ToString());

                    if (newValue == '\r' || (newValue == '\n' && previousValue != '\r'))
                    {
                        Console.WriteLine();
                        await cmdWriter.WriteLineAsync();
                    }
                    else
                    {
                        Console.Write(newValue);
                        await cmdWriter.WriteAsync(newValue);
                    }
                    previousValue = newValue;

                    await Task.Yield();
                }
            });
            Logger.VerboseWriteLine($" done.");

            while (!token.IsCancellationRequested)
            {
                await Task.Delay(100);
            }

            Logger.VerboseWrite($"Closing serial port...");
            _sp.Close();
            Logger.VerboseWriteLine($" done.");
        }

        public void Cancel()
        {
            Logger.VerboseWriteLine();
            Logger.VerboseWriteLine($"Cancellation requested.");
            _cts.Cancel();
            _process.Kill();
        }

        private void Process_Exited(object sender, EventArgs e)
        {
            Logger.VerboseWriteLine($"Command process exited.");
            _cts.Cancel();
        }
    }
}
