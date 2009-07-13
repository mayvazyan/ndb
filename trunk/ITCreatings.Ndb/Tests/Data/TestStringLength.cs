#if TESTS
using ITCreatings.Ndb.Attributes;

namespace ITCreatings.Ndb.Tests.Data
{
    [DbRecord]
    public class TestStringLength
    {
        [DbPrimaryKeyField] public int Id;
        [DbField(2)] public string Text;
    }
}
#endif