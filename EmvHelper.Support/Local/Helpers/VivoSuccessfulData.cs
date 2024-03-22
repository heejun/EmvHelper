using BerTlv;
using System.Collections.Generic;

namespace EmvHelper.Support.Local.Helpers
{
    public class VivoSuccessfulData
    {
        public List<byte[]> Tracks { get; set; } = new List<byte[]> ();
        //public byte[]? Track2 { get; set; }
        //public byte[]? Track3 { get; set; }
        public Tlv? TlvClearing { get; set; }
        public ICollection<Tlv>? TlvData { get; set; }
    }
}
