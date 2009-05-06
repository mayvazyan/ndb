#if DEBUG
using NUnit.Framework;

namespace ITCreatings.Ndb.Tests.MsSql
{
    [TestFixture]
    public class MsSqlDbStructureGatewayTests : DbStructureGatewayTests
    {
        public MsSqlDbStructureGatewayTests() : base(DbAccessor.Create("MsSql")) { }
    }
}
#endif