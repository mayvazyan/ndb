#if DEBUG
using NUnit.Framework;

namespace ITCreatings.Ndb.Tests.Postgre
{
    [TestFixture]
    public class PostgreDbStructureGatewayTests : DbStructureGatewayTests
    {
        public PostgreDbStructureGatewayTests() : base(DbAccessor.Create("Postgre")) { }
    }
}
#endif