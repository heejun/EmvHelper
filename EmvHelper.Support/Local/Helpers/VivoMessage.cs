using BerTlv;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        public VivoSuccessfulData? SuccessfulData { get; private set; }
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
                // Track 1 Length (1)
                // Track 1 Data (var)
                // Track 2 Length (1)
                // Track 2 Data (var)
                // Track 3 Length (1)
                // Track 3 Data (var)
                // DE055(Clearing Record) Present (1) - 01(present), 02(not present)
                // TLV DE055(Clearing Record) (var)
                // TLV Data (var)

                if (vm.Data.Length >= 4)
                {
                    VivoSuccessfulData successfulData = new ();
                    int index = 0;

                    // Track 1
                    for (int i = 0; i < 3; i++)
                    {

                        if (vm.Data[index] > 0)
                        {
                            successfulData.Track1 = new byte[vm.Data[index]];
                            Array.Copy(vm.Data, 0, successfulData.Track1, 0, successfulData.Track1.Length);

                            index += successfulData.Track1.Length + 1;
                        }
                    }
                    if (vm.Data[index] > 0)
                    {
                        successfulData.Track1 = new byte[vm.Data[index]];
                        Array.Copy(vm.Data, 0, successfulData.Track1, 0, successfulData.Track1.Length);

                        index += successfulData.Track1.Length + 1;
                    }

                    // Track 2
                    if (vm.Data[index] > 0)
                    {
                        successfulData.Track2 = new byte[vm.Data[index]];
                        Array.Copy(vm.Data, 0, successfulData.Track2, 0, successfulData.Track2.Length);

                        index += successfulData.Track2.Length + 1;
                    }

                    // Track 3
                    if (vm.Data[index] > 0)
                    {
                        successfulData.Track3 = new byte[vm.Data[index]];
                        Array.Copy(vm.Data, 0, successfulData.Track3, 0, successfulData.Track3.Length);

                        index += successfulData.Track3.Length + 1;
                    }

                    // Clearing Record
                    bool isClearingPresent = (vm.Data[index++] == 1);
                    byte[] temp = new byte[vm.Data.Length - index];
                    Array.Copy(vm.Data, index, temp, 0, temp.Length);
                    ICollection<Tlv> tlvs = Tlv.Parse(temp);

                    if (isClearingPresent)
                    {
                        successfulData.TlvClearing = tlvs.ElementAtOrDefault(0);
                        if (successfulData.TlvClearing != null)
                        {
                            tlvs.Remove(successfulData.TlvClearing);
                        }
                    }

                    // TLV Data
                    successfulData.TlvData = tlvs;

                    vm.SuccessfulData = successfulData;
                }
            }
            else
            {
                if (vm.Data.Length >= 4)
                {
                    VivoFailureData failureData = new ();

                    failureData.ErrorCode = vm.Data[0];
                    failureData.SW1 = vm.Data[1];
                    failureData.SW2 = vm.Data[2];
                    failureData.RfStateCode = vm.Data[3];

                    if (vm.Data.Length > 4)
                    {
                        // https://github.com/kspearrin/BerTlv.NET
                        byte[] tlvData = new byte[vm.Data.Length - 4];
                        Array.Copy(vm.Data, 4, tlvData, 0, tlvData.Length);
                        failureData.TlvData = Tlv.Parse(tlvData);
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
                if (SuccessfulData != null)
                {
                    //sb.Append($"Track 1 Data : {SuccessfulData.Track1?.Length ?? 0}");

                }
            }
            else
            {
                if (FailureData != null)
                {
                    sb.AppendLine($"Error Code : {FailureData.ErrorCode:X2}h");
                    sb.AppendLine($"SW1SW2 : {FailureData.SW1:X2}{FailureData.SW2:X2}h");
                    sb.AppendLine($"RF State Code : {FailureData.RfStateCode:X2}h");

                    if (FailureData.TlvData != null)
                    {
                        foreach (Tlv tlv in FailureData.TlvData)
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
            }

            return sb.ToString();
        }
    }
}
