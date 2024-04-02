using EmvHelper.Support.Local.Helpers.Tags;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace EmvHelper.Support.Local.Helpers
{
    public class TagManager
    {
        private static readonly Dictionary<string, TagInfo> _tagEmvDictionary;
        private static readonly Dictionary<string, TagInfo> _tagVivoDictionary;
        private static readonly Dictionary<string, TagInfo> _tagMcDictionary;
        private static readonly Dictionary<string, TagInfo> _tagVisaDictionary;

        static TagManager()
        {
            _tagEmvDictionary = LoadTagData("Data/emv_common.json");
            _tagVivoDictionary = LoadTagData("Data/emv_vivo.json");
            _tagMcDictionary = LoadTagData("Data/emv_mc.json");
            _tagVisaDictionary = LoadTagData("Data/emv_visa.json");
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
                        result[item.Tag.ToUpper()] = item;
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
            TagInfo? tagInfo;

            if (_tagEmvDictionary.TryGetValue(tag.ToUpper(), out tagInfo))
            {
                return tagInfo;
            }
            else if (_tagVivoDictionary.TryGetValue(tag.ToUpper(), out tagInfo))
            {
                return tagInfo;
            }
            //else if (_tagMcDictionary.TryGetValue(tag.ToUpper(), out tagInfo))
            //{
            //    return tagInfo;
            //}
            else if (_tagVisaDictionary.TryGetValue(tag.ToUpper(), out tagInfo))
            {
                return tagInfo;
            }

            return null;
        }
    }
}
