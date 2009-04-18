#if DEBUG
namespace ITCreatings.Ndb.Tests.Postgre
{  
    public class PostgreDbGatewayTests : DbGatewayTests
    {
        public PostgreDbGatewayTests() : base(DbAccessor.Create("Postgre")) { }
    }

    public class PostgreDbStructureGatewayTests : DbStructureGatewayTests
    {
        public PostgreDbStructureGatewayTests() : base(DbAccessor.Create("Postgre")) { }
    }
}
#endif