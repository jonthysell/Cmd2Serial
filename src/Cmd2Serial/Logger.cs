// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

using System;

namespace Cmd2Serial
{
    public static class Logger
    {
        public static bool VerboseEnabled { get; set; } = false;

        public static event Action<string> LogWrite;

        public static event Action<byte[], int, int> LogWriteBytes;

        public static event Action<byte> LogWriteByte;

        public static void Write(string msg)
        {
            LogWrite?.Invoke(msg);
        }

        public static void WriteLine(string msg = "")
        {
            Write(msg + Environment.NewLine);
        }

        public static void VerboseWrite(string msg)
        {
            if (VerboseEnabled)
            {
                Write(msg);
            }
        }

        public static void VerboseWriteLine(string msg = "")
        {
            if (VerboseEnabled)
            {
                WriteLine(msg);
            }
        }

        public static void WriteBytes(byte[] buffer, int offset, int count)
        {
            LogWriteBytes?.Invoke(buffer, offset, count);
        }

        public static void WriteByte(byte b)
        {
            LogWriteByte?.Invoke(b);
        }
    }
}
