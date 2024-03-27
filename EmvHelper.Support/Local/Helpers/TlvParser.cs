using EmvHelper.Support.Local.Helpers.BerTlv;
using System.Collections.Generic;
using System.Text;

namespace EmvHelper.Support.Local.Helpers
{
    
    public class TlvParser
    {
        public static ICollection<Tlv>? Parse(string tlv)
        {
            try
            {
                return Tlv.Parse(StringHelper.NormalizeString(tlv));
            }
            catch
            {
                return null;
            }

        }

        public static ICollection<Tlv> Parse(byte[] tlv)
        {
            return Tlv.Parse(tlv);
        }

        public static string ToString(Tlv tlv)
        {
            return $"  {tlv.HexTag} : {tlv.Length} : {tlv.HexValue}";
        }

        public static string ToString(ICollection<Tlv> tlvs)
        {
            StringBuilder sb = new();

            if (tlvs != null)
            {
                foreach (Tlv tlv in tlvs)
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

            return sb.ToString();
        }
    }
}
