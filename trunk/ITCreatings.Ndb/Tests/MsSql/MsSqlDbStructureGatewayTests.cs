#if TESTS
using NUnit.Framework;

namespace ITCreatings.Ndb.Tests.MsSql
{
    [TestFixture]
    [Ignore("Just a partially supported now")]
    public class MsSqlDbStructureGatewayTests : DbStructureGatewayTests
    {
        public MsSqlDbStructureGatewayTests() : base(DbAccessor.Create("MsSql")) { }
    }
}
#endif