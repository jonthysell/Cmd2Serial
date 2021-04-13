// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

using System;
using System.Diagnostics;
using System.IO;
using System.Text;
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
                Handshake = config.Handshake,
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
            var cmdOutToSerialTask = StartBridgeAsync(_process.StandardOutput.BaseStream, _sp.BaseStream, Config.CommandToSerialNewLines, true, false, token);
            Logger.VerboseWriteLine($" done.");

            Logger.VerboseWrite($"Linking command errors to serial...");
            var cmdErrToSerialTask = StartBridgeAsync(_process.StandardError.BaseStream, _sp.BaseStream, Config.CommandToSerialNewLines, true, false, token);
            Logger.VerboseWriteLine($" done.");

            Logger.VerboseWrite($"Linking serial to command input...");
            var serialToCmdTask = StartBridgeAsync(_sp.BaseStream, _process.StandardInput.BaseStream, Config.SerialToCommandNewLines, Config.SerialEcho, Config.SerialEcho, token);
            Logger.VerboseWriteLine($" done.");

            while (!token.IsCancellationRequested)
            {
                await Task.Delay(100);
            }

            Logger.VerboseWrite($"Closing process...");
            _process.Close();
            Logger.VerboseWriteLine($" done.");

            Logger.VerboseWrite($"Closing serial port...");
            _sp.Close();
            Logger.VerboseWriteLine($" done.");
        }

        private void Process_Exited(object sender, EventArgs e)
        {
            Logger.VerboseWriteLine($"Command process exited.");
            _cts.Cancel();
        }

        private delegate int ReadByte();
        private delegate Task<int> ReadBytesAsync(byte[] buffer, int offset, int length);
        private delegate void WriteByte(byte b);
        private delegate void WriteBytes(byte[] buffer, int offset, int length);
        private delegate void Flush();

        private async Task StartBridgeAsync(Stream input, Stream output, NewLines newLines, bool logOutput, bool echoInput, CancellationToken token)
        {
            var buffer = new byte[4096];
            int previousByte = -1;

            while (!token.IsCancellationRequested)
            {
                await Task.Yield();

                if (newLines == NewLines.None)
                {
                    int numBytes = await input.ReadAsync(buffer, 0, buffer.Length, token);
                    if (numBytes > 0)
                    {
                        if (echoInput)
                        {
                            input.Write(buffer, 0, numBytes);
                            input.Flush();
                        }
                        output.Write(buffer, 0, numBytes);
                        output.Flush();
                        if (logOutput)
                        {
                            Logger.WriteBytes(buffer, 0, numBytes);
                        }
                    }
                }
                else
                {
                    var byteRead = input.ReadByte();

                    if (byteRead >= 0)
                    {
                        if (byteRead == 0x0D || (byteRead == 0x0A && previousByte != 0x0D))
                        {
                            switch (newLines)
                            {
                                case NewLines.CR:
                                    if (echoInput)
                                    {
                                        input.WriteByte(0x0D);
                                        input.Flush();
                                    }
                                    output.WriteByte(0x0D);
                                    output.Flush();
                                    if (logOutput)
                                    {
                                        Logger.WriteByte(0x0D);
                                    }
                                    break;
                                case NewLines.LF:
                                    if (echoInput)
                                    {
                                        input.WriteByte(0x0A);
                                        input.Flush();
                                    }
                                    output.WriteByte(0x0A);
                                    output.Flush();
                                    if (logOutput)
                                    {
                                        Logger.WriteByte(0x0A);
                                    }
                                    break;
                                case NewLines.CRLF:
                                    if (echoInput)
                                    {
                                        input.WriteByte(0x0D);
                                        input.WriteByte(0x0A);
                                        input.Flush();
                                    }
                                    output.WriteByte(0x0D);
                                    output.WriteByte(0x0A);
                                    output.Flush();
                                    if (logOutput)
                                    {
                                        Logger.WriteByte(0x0D);
                                        Logger.WriteByte(0x0A);
                                    }
                                    break;
                            }
                        }
                        else
                        {
                            if (echoInput)
                            {
                                input.WriteByte((byte)byteRead);
                                input.Flush();
                            }
                            output.WriteByte((byte)byteRead);
                            output.Flush();
                            if (logOutput)
                            {
                                Logger.WriteByte((byte)byteRead);
                            }
                        }
                    }

                    previousByte = byteRead;
                }
            }
        }

        public void Cancel()
        {
            Logger.VerboseWriteLine();
            Logger.VerboseWriteLine($"Cancellation requested.");
            _cts.Cancel();
            _process.Kill();
        }
    }
}
