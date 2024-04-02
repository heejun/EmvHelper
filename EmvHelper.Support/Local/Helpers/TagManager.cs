using EmvHelper.Support.Local.Helpers.Tags;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.IO;
using System.Linq;
using System.Transactions;
using YamlDotNet.Core.Tokens;

namespace EmvHelper.Support.Local.Helpers
{
    public class TagManager
    {
        private static readonly Dictionary<string, TagInfo> _TagDictionary;

        static TagManager()
        {
            _TagDictionary = LoadTagData("Data/tags_emv.json");
        }

        private static Dictionary<string, TagInfo> LoadTagData(string filePath)
        {
            Dictionary<string, TagInfo> result = new();

            try
            {
                string jsonText = File.ReadAllText(filePath);

                List<TagInfo>? dataList = JsonConvert.DeserializeObject<List<TagInfo>>(jsonText);

                if (dataList is not null)
                {
                    foreach (var item in dataList)
                    {
                        result[item.Tag] = item;
                    }
                }

            }
            catch
            {
            }

            return result;
        }

        public static TagInfo? GetTagInfo(string tag)
        {
            if (_TagDictionary.TryGetValue(tag, out TagInfo? tagInfo))
            {
                return tagInfo;
            }

            return null;
        }
    }
}
