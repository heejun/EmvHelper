using EmvHelper.Support.Local.Helpers.Tags;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Transactions;

namespace EmvHelper.Support.Local.Helpers
{
    public class TagManager
    {
        private static List<TagInfo> _tagList = new();

        static TagManager()
        {
            LoadTagDescriptions();
        }

        private static void LoadTagDescriptions()
        {
            //9F 1E 08 37 33 31 54 38 32 36 33 9F 41 04 00 00 04 18 DF EE 4C 01 00 9F 2A 00 DF EC 1C 00

            _tagList.Add(new TagInfo("9F1E", "Interface Device (IFD) Serial Number", "Unique and permanent serial number assigned to the IFD by the manufacturer"));
            _tagList.Add(new TagInfo("9F41", "Transaction Sequence Counter", "Counter maintained by the terminal that is incremented by one for each transaction"));
            _tagList.Add(new TagInfo("DFEE4C", "[Vivo]Selected Application", ""));
            _tagList.Add(new TagInfo("9F2A", "[Vivo]Kernel ID", ""));
            _tagList.Add(new TagInfo("DFEC1C", "[Vivo]XFS Format Data", ""));

        }

        public static TagInfo? GetTagInfo(string tag)
        {
            return _tagList.FirstOrDefault(x => x.Tag == tag);
        }


    }
}
