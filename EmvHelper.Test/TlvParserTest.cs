using EmvHelper.Support.Local.Helpers;
using EmvHelper.Support.Local.Helpers.BerTlv;

namespace EmvHelper.Test
{
    public class TlvParserTest
    {
        [Fact]
        public void Parse_OneTlv()
        {
            string tlv = "9F 1E 08 37 33 31 54 38 32 36 33";
            //   Tlv data      : 9F 1E 08 37 33 31 54 38 32 36 33 9F 41 04 00 00 04 18 DF EE 4C 01 00 9F 2A 00 DF EC 1C 00 7F B6

            ICollection<Tlv>? tlvs = TlvParser.Parse(tlv);
            
            Assert.NotNull(tlvs);
            Assert.Equal("9F1E", tlvs.First().HexTag);
            Assert.Equal("8", tlvs.First().HexLength);
            Assert.Equal("3733315438323633", tlvs.First().HexValue);
        }

        [Fact]
        public void Parse_MultipleTlvs()
        {
            string tlv = "9F 1E 08 37 33 31 54 38 32 36 33 9F 41 04 00 00 04 18 DF EE 4C 01 00 9F 2A 00 DF EC 1C 00";

            ICollection<Tlv>? tlvs = TlvParser.Parse(tlv);

            Assert.NotNull(tlvs);
        }

        [Fact]
        public void Parse_MultipleTlvsWithInvalidData()
        {
            string tlv = "9F 1E 08 37 33 31 54 38 32 36 33 AB CD EF";

            ICollection<Tlv>? tlvs = TlvParser.Parse(tlv);

            Assert.NotNull(tlvs);
            Assert.Equal(1, tlvs.Count);
            Assert.Equal("9F1E", tlvs.First().HexTag);
            Assert.Equal("8", tlvs.First().HexLength);
            Assert.Equal("3733315438323633", tlvs.First().HexValue);
        }
    }
}