#if DEBUG
using ITCreatings.Ndb.Exceptions;
using ITCreatings.Ndb.Tests.Data;
using NUnit.Framework;

namespace ITCreatings.Ndb.Tests.Postgre
{
    [TestFixture]
    public class PostgreDbGatewayTests : DbGatewayTests
    {
        public PostgreDbGatewayTests() : base(DbAccessor.Create("Postgre")) { }

        [Test]
        [ExpectedException(typeof(NdbException))]
        public void FieldSizeTest()
        {
            TestStringLength record = new TestStringLength { Text = "123" };
            Assert.IsTrue(DbTestUtils.SaveTest(record));

            TestStringLength load = gateway.Load<TestStringLength>(record.Id);
            Assert.AreEqual("12", load.Text);
        }

    }
}
#endif