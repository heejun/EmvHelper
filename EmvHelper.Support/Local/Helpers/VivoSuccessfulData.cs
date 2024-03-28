using System.Collections.Generic;

namespace EmvHelper.Support.Local.Helpers
{
    public class VivoSuccessfulData
    {
        public List<byte[]?> Tracks { get; set; } = new List<byte[]?> { null, null, null };
    }
}
