using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmvHelper.Support.Local.Helpers
{
    public class ActivateTransactionResponseData
    {
        public byte[]? Track1Data { get; private set; }
        public byte[]? Track2Data { get; private set; }
        public byte[]? Track3Data { get; private set; }

        public bool IsClearingRecordPresent { get; private set; }
        public byte[]? ClearingRecord { get; private set; }

        public byte[]? TlvData { get; private set; }
    }
}
