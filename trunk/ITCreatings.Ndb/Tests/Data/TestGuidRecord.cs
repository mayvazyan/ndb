#if TESTS
using System;
using ITCreatings.Ndb.Attributes;

namespace ITCreatings.Ndb.Tests.Data
{
    [DbRecord]
    public class TestGuidRecord
    {
        [DbPrimaryKeyField] public Guid Guid;
        [DbField] public string Title;
        [DbField] public Guid TestGuidField;
    }
}
#endif