#if DEBUG
using NUnit.Framework;

namespace ITCreatings.Ndb.Tests.MsSql
{
    [TestFixture]
    public class MsSqlDbGatewayTests : DbGatewayTests
    {
        public MsSqlDbGatewayTests() : base(DbAccessor.Create("MsSql")) { }
    }
}
#endif