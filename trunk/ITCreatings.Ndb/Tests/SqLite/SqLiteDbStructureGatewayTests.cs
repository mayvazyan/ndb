#if DEBUG
using NUnit.Framework;

namespace ITCreatings.Ndb.Tests.SqLite
{
    [TestFixture]
    public class SqLiteDbStructureGatewayTests : DbStructureGatewayTests
    {
        public SqLiteDbStructureGatewayTests() : base(DbAccessor.Create("SqLite")) { }

        [ExpectedException(typeof(System.NotImplementedException))]
        public override void AlterTableTest()
        {
            base.AlterTableTest();
        }
    }
}
#endif