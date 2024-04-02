using EmvHelper.Support.Local.Helpers.BerTlv;
using EmvHelper.Support.Local.Helpers.Tags;
using System.Collections.Generic;
using System.Text;

namespace EmvHelper.Support.Local.Helpers
{
    public class TlvParser
    {
        public static ICollection<Tlv> Parse(string tlv)
        {
            return Tlv.Parse(StringHelper.NormalizeString(tlv));
        }

        public static ICollection<Tlv> Parse(byte[] tlv)
        {
            return Tlv.Parse(tlv);
        }

        public static string ToString(Tlv tlv)
        {
            return ToString(new List<Tlv>() { tlv });
        }

        public static string ToString(ICollection<Tlv> tlvs, int depth = 0)
        {
            StringBuilder sb = new();
            int numOfSpaces = 4;

            if (tlvs != null)
            {
                foreach (Tlv tlv in tlvs)
                {
                    TagInfo? tagInfo = TagManager.GetTagInfo(tlv.HexTag);
                    sb.AppendLine($"{new string(' ', numOfSpaces * depth)}{tlv.HexTag} ({tagInfo?.Name ?? "*"}) : {tlv.HexLength} : {tlv.HexValue}");
                    if (tlv.Children != null)
                    {
                        ToString(tlv.Children, depth + 1);
                    }
                }
            }

            return sb.ToString();
        }
    }
}
