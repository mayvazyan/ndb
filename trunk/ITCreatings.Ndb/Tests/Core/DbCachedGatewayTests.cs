#if TESTS
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
            DbCachedGateway cachedGateway = new DbCachedGateway(DbGateway.Instance);
            const string KEY = "key";

            Assert.IsNull(DbCachedGateway.Cache.Get(KEY));

            var list = cachedGateway.LoadList<User>(KEY);
            Assert.Less(0, list.Length);

            Assert.IsNotNull(DbCachedGateway.Cache.Get(KEY));
            var list2 = cachedGateway.LoadList<User>(KEY);
            Assert.Less(list.Length, list2.Length);
        }
    }
}

#endif