#if DEBUG
using ITCreatings.Ndb.Tests.Data;
using NUnit.Framework;

namespace ITCreatings.Ndb.Tests.Core
{
    [TestFixture]
    [Explicit("Seems doesn't work under Vista 64 bit")]
    public class DbCachedGatewayTests : DbTestFixture
    {
        [Test]
        public void LoadListTest()
        {
            DbCachedGateway gateway = new DbCachedGateway(DbGateway.Instance);
            const string key = "key";

            Assert.IsNull(DbCachedGateway.Cache.Get(key));

            var list = gateway.LoadList<TestUser>(key);
            Assert.Less(0, list.Length);

            Assert.IsNotNull(DbCachedGateway.Cache.Get(key));
        }
    }
}

#endif