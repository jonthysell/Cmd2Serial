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

        #region Command

        public string Command = @"";

        public string CommandArgs = @"";

        public string FullCommand => Command + (!string.IsNullOrWhiteSpace(CommandArgs) ? " " + CommandArgs : "");

        #endregion
    }
}
