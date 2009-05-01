#if DEBUG
using ITCreatings.Ndb.Attributes;

namespace ITCreatings.Ndb.Tests.Data
{
    [DbRecord]
    public class BinaryDataRecord
    {
        [DbPrimaryKeyField] public uint Id;

        [DbField] public byte[] Data;
    }
}
#endif