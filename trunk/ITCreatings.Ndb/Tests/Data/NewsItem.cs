#if TESTS
using ITCreatings.Ndb.Attributes;

namespace ITCreatings.Ndb.Tests.Data
{
    [DbRecord("News")]
    public class NewsItem
    {
        [DbPrimaryKeyField] public int Id;
        [DbField] public string Title;
        [DbField] public int Age;
    }

    [DbRecord("News")]
    public class NewsItem2
    {
        [DbPrimaryKeyField]
        public long Id;
        [DbField]
        public string Title;
        [DbField]
        public byte Age;
        [DbField]
        public int CategoryId;
        [DbField]
        public string Content;
    }


    [DbRecord("NewsCat")]
    public class NewsItemCat
    {
        [DbPrimaryKeyField] public int Id;
        [DbField] public string Title;
        [DbField] public int Age;

        [DbField] public int CategoryId;
        [DbField] public string Content;
    }
    
    [DbRecord("NewsCat")]
    public class NewsItemCatChild : NewsItem
    {
        [DbField] public int CategoryId;
        [DbField] public string Content;
    }
}
#endif