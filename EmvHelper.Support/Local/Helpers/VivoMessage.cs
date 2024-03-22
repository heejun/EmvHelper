using BerTlv;
using System;
using System.Text;

namespace EmvHelper.Support.Local.Helpers
{
    public class VivoMessage
    {
        public bool IsValidMessage { get; private set; } = false;

        public VivoMessageType MessageType { get; private set; }
        public byte[]? HeaderTag { get; private set; } = null;
        public byte Command { get; private set; }
        public byte SubCommand { get; private set; }
        public byte StatusCode { get; private set; }
        public byte[]? Data { get; private set; } = null;

        public bool IsSuccessfulTransaction => (StatusCode == 0x00 || StatusCode == 0x23);

        public VivoFailureData? FailureData { get; private set; }

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
            int dataLength = message[12] << 8 | message[13];

            // Check if the message contains enough bytes for the data
            if (message.Length < 14 + dataLength)
            {
                return null; // Message does not contain as much data as specified
            }

            if (dataLength > 0)
            {
                // Parse data (bytes 14 onwards, length specified by DataLength)
                vivoMessage.Data = new byte[dataLength];
                Array.Copy(message, 14, vivoMessage.Data, 0, dataLength);

                ParseDataField(vivoMessage);
            }

            // Parsing successful
            vivoMessage.IsValidMessage = true;

            return vivoMessage;
        }

        private static void ParseDataField(VivoMessage vm)
        {
            if (vm.Data == null)
            {
                return;
            }

            if (vm.IsSuccessfulTransaction)
            {

            }
            else
            {
                if (vm.Data.Length >= 4)
                {
                    VivoFailureData failureData = new VivoFailureData();

                    failureData.ErrorCode = vm.Data[0];
                    failureData.SW1 = vm.Data[1];
                    failureData.SW2 = vm.Data[2];
                    failureData.RfStateCode = vm.Data[3];

                    if (vm.Data.Length > 4)
                    {
                        // https://github.com/kspearrin/BerTlv.NET
                        byte[] tlvData = new byte[vm.Data.Length - 4];
                        Array.Copy(vm.Data, 4, tlvData, 0, tlvData.Length);
                        failureData.Tlvs = Tlv.Parse(tlvData);
                    }

                    vm.FailureData = failureData;
                }
            }
        }

        public override string ToString()
        {
            if (!IsValidMessage)
            {
                return "Invalid Message";
            }

            StringBuilder sb = new();
            sb.AppendLine($"Header : {VivoParser.ByteArrayToAsciiString(HeaderTag)}");
            sb.AppendLine($"Command : {Command:X2}h");
            sb.AppendLine($"Status Code : {StatusCode:X2}h");
            sb.AppendLine($"Data Length : {Data?.Length ?? 0}");
            string hexString = VivoParser.ByteArrayToHexString(Data);
            if (hexString != null)
            {
                sb.AppendLine($"Data : {hexString}");
            }

            if (IsSuccessfulTransaction)
            {

            }
            else
            {
                if (FailureData != null)
                {
                    sb.AppendLine($"Error Code : {FailureData.ErrorCode:X2}h");
                    sb.AppendLine($"SW1SW2 : {FailureData.SW1:X2}{FailureData.SW2:X2}h");
                    sb.AppendLine($"RF State Code : {FailureData.RfStateCode:X2}h");
                    foreach (Tlv tlv in FailureData.Tlvs)
                    {
                        sb.AppendLine($"  {tlv.HexTag} : {tlv.Length} : {tlv.HexValue}");
                        if (tlv.Children != null)
                        {
                            foreach (Tlv childTlv in tlv.Children)
                            {
                                sb.AppendLine($"    {childTlv.HexTag} : {childTlv.Length} : {childTlv.HexValue}");
                            }
                        }
                    }
                }

            }

            return sb.ToString();
        }
    }
}
