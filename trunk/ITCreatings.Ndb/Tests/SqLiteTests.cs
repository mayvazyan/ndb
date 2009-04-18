#if DEBUG
namespace ITCreatings.Ndb.Tests.SqLite
{  
    public class SqLiteDbGatewayTests : DbGatewayTests
    {
        public SqLiteDbGatewayTests() : base(DbAccessor.Create("SqLite")) { }
    }

    public class SqLiteDbStructureGatewayTests : DbStructureGatewayTests
    {
        public SqLiteDbStructureGatewayTests() : base(DbAccessor.Create("SqLite")) { }

        [NUnit.Framework.ExpectedException(typeof(System.NotImplementedException))]
        public override void AlterTableTest()
        {
            base.AlterTableTest();
        }
    }
}
#endif