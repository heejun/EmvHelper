using EmvHelper.Support.Local.Helpers.BerTlv;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EmvHelper.Support.Local.Helpers
{
    public class VivoMessage
    {
        private static readonly Dictionary<int, string> _statusDictionary;
        private static readonly Dictionary<int, string> _errorDictionary;
        private static readonly Dictionary<int, string> _rfStateDictionary;

        static VivoMessage()
        {
            _statusDictionary = LoadData("Data/status_codes.json");
            _errorDictionary = LoadData("Data/error_codes.json");
            _rfStateDictionary = LoadData("Data/rfstate_codes.json");
        }

        private static Dictionary<int, string> LoadData(string filePath)
        {
            Dictionary<int, string> result = new();

            try
            {
                string jsonText = File.ReadAllText(filePath);

                JObject jsonObject = JObject.Parse(jsonText);

                foreach (var item in jsonObject)
                {
                    result.Add(Convert.ToInt32(item.Key, 16), (item.Value ?? "").ToString());
                }
            }
            catch
            {
            }

            return result;
        }

        static TValue GetValueOrDefault<TKey, TValue>(Dictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
        {
            if (dictionary.TryGetValue(key, out TValue? value))
            {
                return value; // 해당 키에 해당되는 값이 있으면 반환
            }
            else
            {
                return defaultValue; // 해당 키에 해당되는 값이 없으면 기본값 반환
            }
        }

        public bool IsValidMessage { get; private set; } = false;

        public VivoMessageType MessageType { get; private set; }
        public byte[]? HeaderTag { get; private set; } = null;
        public byte Command { get; private set; }
        public byte SubCommand { get; private set; }
        public byte StatusCode { get; private set; }
        public byte[]? Data { get; private set; } = null;
        public VivoSuccessfulData? SuccessfulData { get; private set; }
        public VivoFailureData? FailureData { get; private set; }
        public byte[]? TlvRawData { get; private set; }
        public ICollection<Tlv>? TlvData { get; private set; }

        public bool IsSuccessfulTransaction => (StatusCode == 0x00 || StatusCode == 0x23);

        public static VivoMessage? Parse(string message, VivoMessageType messageType)
        {
            byte[]? bytes = StringHelper.HexStringToByteArray(StringHelper.NormalizeString(message));
            if (bytes == null)
            {
                return null;
            }

            return Parse(bytes, messageType);
        }

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
                    VivoSuccessfulData successfulData = new();
                    int index = 0;

                    // Track 1/2/3
                    for (int i = 0; i < 3; i++)
                    {
                        var trackLength = vm.Data[index++];

                        if (trackLength > 0)
                        {
                            var trackData = new byte[trackLength];
                            Array.Copy(vm.Data, index, trackData, 0, trackData.Length);
                            successfulData.Tracks[i] = trackData;
                            index += trackData.Length;
                        }
                    }

                    // Clearing Record Present
                    //bool isClearingPresent = (vm.Data[index++] == 1);

                    vm.TlvRawData = new byte[vm.Data.Length - index];
                    Array.Copy(vm.Data, index, vm.TlvRawData, 0, vm.TlvRawData.Length);
                    vm.TlvData = TlvParser.Parse(vm.TlvRawData);

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
                        vm.TlvRawData = new byte[vm.Data.Length - 4];
                        Array.Copy(vm.Data, 4, vm.TlvRawData, 0, vm.TlvRawData.Length);
                        vm.TlvData = TlvParser.Parse(vm.TlvRawData);
                    }

                    vm.FailureData = failureData;
                }
            }
        }

        public string ToString(CardBrandType brandType)
        {
            if (!IsValidMessage)
            {
                return "Invalid Message";
            }

            StringBuilder sb = new();
            sb.AppendLine($"Header : {StringHelper.ByteArrayToAsciiString(HeaderTag)}");
            sb.AppendLine($"Command : {Command:X2}h");
            sb.AppendLine($"Status Code : {StatusCode:X2}h - {GetValueOrDefault(_statusDictionary, StatusCode, "")}");
            sb.AppendLine($"Data Length : {(Data?.Length ?? 0):X2}h");
            string hexString = StringHelper.ByteArrayToHexString(Data);
            if (hexString != null)
            {
                sb.AppendLine($"Data : {hexString}");
            }

            if (IsSuccessfulTransaction)
            {
                if (SuccessfulData != null)
                {
                    // Track 1/2/3
                    for (int i = 0; i < 3; i++)
                    {
                        var trackData = SuccessfulData.Tracks[i];
                        sb.Append($"Track {i + 1} Data : {(trackData?.Length ?? 0):X2}h : ");
                        if (trackData != null)
                        {
                            sb.AppendLine($"{StringHelper.ByteArrayToHexString(trackData)}");
                        }
                        else
                        {
                            sb.AppendLine();
                        }
                    }
                }
            }
            else
            {
                if (FailureData != null)
                {
                    sb.AppendLine($"Error Code : {FailureData.ErrorCode:X2}h - {GetValueOrDefault(_errorDictionary, FailureData.ErrorCode, "")}");
                    sb.AppendLine($"SW1SW2 : {FailureData.SW1:X2}{FailureData.SW2:X2}h");
                    sb.AppendLine($"RF State Code : {FailureData.RfStateCode:X2}h - {GetValueOrDefault(_rfStateDictionary, FailureData.RfStateCode, "")}");
                }
            }

            if (TlvData != null)
            {
                sb.AppendLine($"TLV Data : {StringHelper.ByteArrayToHexString(TlvRawData)}");
                sb.AppendLine(TlvParser.ToString(TlvData, brandType, 1));
            }

            return sb.ToString();
        }
    }
}
