#if DEBUG
using NUnit.Framework;

namespace ITCreatings.Ndb.Tests.MsSql
{
    [TestFixture]
    [Ignore("Just a partially supported now")]
    public class MsSqlDbGatewayTests : DbGatewayTests
    {
        public MsSqlDbGatewayTests() : base(DbAccessor.Create("MsSql")) { }
    }
}
#endif