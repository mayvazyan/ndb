#if DEBUG
using ITCreatings.Ndb.Core;
using ITCreatings.Ndb.Tests.Data;
using NUnit.Framework;

namespace ITCreatings.Ndb.Tests.Core
{
    [TestFixture]
    public class DbTableCheckResultTest : DbTestFixture
    {
        [Test]
        public void Maintest()
        {
            DbStructureGateway g = DbStructureGateway.Instance;
            g.CreateTable(typeof(NewsItem));
            Assert.IsTrue(g.IsValid(typeof(NewsItem)), g.LastError);
            Assert.IsFalse(g.IsValid(typeof(NewsItem2)));


            DbTableCheckResult result = new DbTableCheckResult(DbAccessor.Instance);
            result.Build(typeof(NewsItem2));

            Assert.AreEqual(2, result.FieldsToCreate.Count);
            Assert.AreEqual(1, result.FieldsToUpdate.Count);


            g.DropTable(typeof(NewsItem));
            g.CreateTable(typeof(NewsItem2));
            Assert.IsTrue(g.IsValid(typeof(NewsItem2)));
            Assert.IsFalse(g.IsValid(typeof(NewsItem)));


            result = new DbTableCheckResult(DbAccessor.Instance);
            result.Build(typeof(NewsItem2));

            Assert.AreEqual(0, result.FieldsToCreate.Count);
            Assert.AreEqual(0, result.FieldsToUpdate.Count);

        }
    }
}
#endif