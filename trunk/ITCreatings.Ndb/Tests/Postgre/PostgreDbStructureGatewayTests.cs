#if DEBUG
using NUnit.Framework;

namespace ITCreatings.Ndb.Tests.Postgre
{
    [TestFixture]
    [Ignore("Just a partially supported now")]
    public class PostgreDbStructureGatewayTests : DbStructureGatewayTests
    {
        public PostgreDbStructureGatewayTests() : base(DbAccessor.Create("Postgre")) { }
    }
}
#endif