#if DEBUG
using NUnit.Framework;

namespace ITCreatings.Ndb.Tests.SqLite
{
    [TestFixture]
    public class SqLiteDbGatewayTests : DbGatewayTests
    {
        public SqLiteDbGatewayTests() : base(DbAccessor.Create("SqLite")) { }
    }
}
#endif