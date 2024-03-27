using EmvHelper.Support.Local.Helpers.BerTlv;
using System.Collections.Generic;

namespace EmvHelper.Support.Local.Helpers
{
    public class HjParser
    {
        public static ICollection<Tlv> ParseTlv(string tlv)
        {
            return TlvParser.Parse(tlv);
        }

        public static VivoMessage? ParseVivoMessage(string message, VivoMessageType messageType)
        {
            return VivoMessage.Parse(message, messageType);
        }
    }
}
