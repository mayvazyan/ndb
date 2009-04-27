#if DEBUG
using System;
using ITCreatings.Ndb.Attributes;
using ITCreatings.Ndb.Attributes.Keys;

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