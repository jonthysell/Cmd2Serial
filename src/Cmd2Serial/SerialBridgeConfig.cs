// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

using System.IO.Ports;

namespace Cmd2Serial
{
    public class SerialBridgeConfig
    {
        #region Serial Port

        public string PortName = "";

        public int BaudRate = 9600;

        public Parity Parity = Parity.None;

        public int DataBits = 8;

        public StopBits StopBits = StopBits.One;

        public Handshake Handshake = Handshake.None;

        #endregion

        public NewLines CommandToSerialNewLines = NewLines.None;

        public NewLines SerialToCommandNewLines = NewLines.None;

        public bool SerialEcho = false;        

        public bool RedirectOutput = true;

        public bool RedirectError = true;

        public bool RedirectInput = true;

        #region Command

        public string Command = @"";

        public string CommandArgs = @"";

        public string FullCommand => Command + (!string.IsNullOrWhiteSpace(CommandArgs) ? " " + CommandArgs : "");

        #endregion
    }

    public enum NewLines
    {
        None,
        CR,
        LF,
        CRLF,
    }
}
