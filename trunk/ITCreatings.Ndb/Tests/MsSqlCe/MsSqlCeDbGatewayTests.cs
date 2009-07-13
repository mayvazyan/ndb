#if TESTS
using NUnit.Framework;

namespace ITCreatings.Ndb.Tests.MsSqlCe
{
    [TestFixture]
    [Ignore("not finished")]
    public class MsSqlCeDbGatewayTests : DbGatewayTests
    {
        public MsSqlCeDbGatewayTests() : base(DbAccessor.Create("MsSqlCe")) { }
    }
}
#endif