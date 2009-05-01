#if DEBUG
using ITCreatings.Ndb.Attributes;

namespace ITCreatings.Ndb.Tests.Data
{
    [DbRecord("News")]
    public class NewsItem
    {
        [DbPrimaryKeyField] public uint Id;
        [DbField] public string Title;
        [DbField] public uint Age;
    }

    [DbRecord("News")]
    public class NewsItem2
    {
        [DbPrimaryKeyField]
        public ulong Id;
        [DbField]
        public string Title;
        [DbField]
        public byte Age;
        [DbField]
        public uint CategoryId;
        [DbField]
        public string Content;
    }


    [DbRecord("NewsCat")]
    public class NewsItemCat
    {
        [DbPrimaryKeyField] public uint Id;
        [DbField] public string Title;
        [DbField] public uint Age;

        [DbField] public uint CategoryId;
        [DbField] public string Content;
    }
    
    [DbRecord("NewsCat")]
    public class NewsItemCatChild : NewsItem
    {
        [DbField] public uint CategoryId;
        [DbField] public string Content;
    }
}
#endif