#if TESTS
using ITCreatings.Ndb.Attributes;

namespace ITCreatings.Ndb.Tests.Data
{
    [DbRecord("BinaryDataRecords")]
    public class BinaryDataRecord3
    {
        [DbPrimaryKeyField]
        public int Id;

        [DbField(typeof(byte[]))]
        public string Data;
    }
}
#endif