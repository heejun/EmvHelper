using System;
using System.Text;
using System.Text.RegularExpressions;

namespace EmvHelper.Support.Local.Helpers
{
    public class VivoParser
    {
        public static VivoMessage? Parse(string input, VivoMessageType messageType)
        {
            byte[]? data = VivoParser.HexStringToByteArray(VivoParser.Normalize(input));
            if (data == null)
            {
                return null;
            }

            VivoMessage? vivoMessage = VivoMessage.Parse(data, messageType);
            return vivoMessage;
        }

        private static string Normalize(string input)
        {
            string output = Regex.Replace(input, @"\s+", "").ToUpper();

            if (string.IsNullOrEmpty(output))
            {
                return string.Empty;
            }

            if (!Regex.IsMatch(output, "^[0-9A-F]+$"))
            {
                return string.Empty;
            }

            return output;
        }

        public static byte[]? HexStringToByteArray(string hex)
        {
            if (hex.Length % 2 != 0)
            {
                return null;
            }

            byte[] bytes = new byte[hex.Length / 2];
            for (int i = 0; i < hex.Length; i += 2)
            {
                bytes[i/2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }

            return bytes;
        }

        public static string ByteArrayToHexString(byte[]? byteArray)
        {
            if (byteArray == null)
            {
                return string.Empty;
            }

            return Convert.ToHexString(byteArray);
        }

        public static string ByteArrayToAsciiString(byte[]? byteArray)
        {
            if (byteArray == null)
            {
                return string.Empty;
            }

            return Encoding.ASCII.GetString(byteArray);
        }
    }
}
