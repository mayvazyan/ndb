#if TESTS
using ITCreatings.Ndb.Attributes;

namespace ITCreatings.Ndb.Tests.Data
{
    [DbRecord("BinaryDataRecords")]
    public class BinaryDataRecord2
    {
        [DbPrimaryKeyField]
        public int Id;

        [DbField(typeof(byte[]))]
        public int Data;
    }
}
#endif