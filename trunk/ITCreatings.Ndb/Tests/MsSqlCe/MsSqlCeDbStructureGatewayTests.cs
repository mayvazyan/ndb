#if DEBUG
using NUnit.Framework;

namespace ITCreatings.Ndb.Tests.MsSqlCe
{
    [TestFixture]
    [Ignore("not finished")]
    public class MsSqlCeDbStructureGatewayTests : DbStructureGatewayTests
    {
        public MsSqlCeDbStructureGatewayTests() : base(DbAccessor.Create("MsSqlCe")) { }
    }
}
#endif