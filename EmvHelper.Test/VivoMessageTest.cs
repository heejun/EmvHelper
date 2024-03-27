using EmvHelper.Support.Local.Helpers;

namespace EmvHelper.Test
{
    public class VivoMessageTest
    {
        [Fact]
        public void Parse_FailedResponse1()
        {
            string respMessage = "56 69 56 4F 74 65 63 68 32 00 02 0A 00 22 27 90 00 01 9F 1E 08 37 33 31 54 38 32 36 33 9F 41 04 00 00 04 18 DF EE 4C 01 00 9F 2A 00 DF EC 1C 00 7F B6";
            // Header      : 56 69 56 4F 74 65 63 68 32 00
            // Command     : 02
            // Status code : 0A
            // Data length : 00 22
            // Data        :
            //   Error code    : 27
            //   SW1SW2        : 90 00
            //   RF State code : 01
            //   Tlv data      : 9F 1E 08 37 33 31 54 38 32 36 33 9F 41 04 00 00 04 18 DF EE 4C 01 00 9F 2A 00 DF EC 1C 00 7F B6

            var result = HjParser.ParseVivoMessage(respMessage, VivoMessageType.Response);
            Assert.NotNull(result);
            Assert.True(result.IsValidMessage);
            Assert.NotNull(result.HeaderTag);
            Assert.True(Convert.ToHexString(result.HeaderTag).Equals("56 69 56 4F 74 65 63 68 32 00".Replace(" ", "")));
            Assert.True(result.Command == 0x02);
            Assert.True(result.StatusCode == 0x0A);
            Assert.True(result.Data?.Length == 0x0022);
            Assert.True(!result.IsSuccessfulTransaction);
            Assert.NotNull(result.FailureData);
            Assert.True(result.FailureData.ErrorCode == 0x27);
            Assert.True(result.FailureData.SW1 == 0x90);
            Assert.True(result.FailureData.SW2 == 0x00);
            Assert.True(result.FailureData.RfStateCode == 0x01);
        }
    }
}