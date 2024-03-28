namespace EmvHelper.Support.Local.Helpers.Tags
{
    public class TagInfo
    {
        public string Tag { get; }
        public string Name { get; }
        public string Desc { get; }

        public TagInfo(string tag, string name, string desc)
        {
            Tag = tag;
            Name = name;
            Desc = desc;
        }
    }
}
