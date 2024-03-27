using EmvHelper.Support.Local.Helpers.BerTlv;
using System.Collections.Generic;

namespace EmvHelper.Support.Local.Helpers
{
    public class VivoFailureData
    {
        public byte ErrorCode { get; set; }
        public byte SW1 { get; set; }
        public byte SW2 { get; set; }
        public byte RfStateCode { get; set; }
        public ICollection<Tlv>? TlvData { get; set; }
    }
}
