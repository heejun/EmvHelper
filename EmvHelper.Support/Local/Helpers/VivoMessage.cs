using System;
using System.Text;

namespace EmvHelper.Support.Local.Helpers
{
    public class VivoMessage
    {
        public bool IsValid { get; private set; } = false;
        public VivoMessageType MessageType { get; private set; }
        public byte[]? HeaderTag { get; private set; } = null;
        public byte Command { get; private set; }
        public byte SubCommand { get; private set; }
        public byte StatusCode { get; private set; }
        public int DataLength { get; private set; }
        public byte[]? Data { get; private set; } = null;

        public static VivoMessage? Parse(byte[] message, VivoMessageType messageType)
        {
            if (message == null || message.Length < 14)
            {
                return null; // Message is too short to contain all required parts
            }

            VivoMessage vivoMessage = new ();

            vivoMessage.MessageType = messageType;

            // Parse header tag (bytes 0-9)
            vivoMessage.HeaderTag = new byte[10];
            Array.Copy(message, 0, vivoMessage.HeaderTag, 0, 10);

            // Parse command (byte 10)
            vivoMessage.Command = message[10];

            if (messageType == VivoMessageType.Command)
            {
                // Parse sub-command code (byte 11)
                vivoMessage.SubCommand = message[11];
            }
            else
            {
                // Parse status code (byte 11)
                vivoMessage.StatusCode = message[11];
            }

            // Parse data length (bytes 12 and 13)
            vivoMessage.DataLength = message[12] << 8 | message[13];

            // Check if the message contains enough bytes for the data
            if (message.Length < 14 + vivoMessage.DataLength)
            {
                return null; // Message does not contain as much data as specified
            }

            // Parse data (bytes 14 onwards, length specified by DataLength)
            vivoMessage.Data = new byte[vivoMessage.DataLength];
            Array.Copy(message, 14, vivoMessage.Data, 0, vivoMessage.DataLength);

            // Parsing successful
            vivoMessage.IsValid = true;
            return vivoMessage;
        }

        public override string ToString()
        {
            if (!IsValid)
            {
                return "Invalid Message";
            }

            StringBuilder sb = new();
            sb.AppendLine($"Header : {VivoParser.ByteArrayToAsciiString(HeaderTag)}");
            sb.AppendLine($"Command : {Command:X2}h");
            sb.AppendLine($"Status Code : {StatusCode:X2}h");
            sb.AppendLine($"Data Length : {DataLength}");
            sb.AppendLine($"Data : {VivoParser.ByteArrayToHexString(Data)}");

            return sb.ToString();
        }
    }
}
