#if DEBUG

using System;
using ITCreatings.Ndb.Tests.Data;
using NUnit.Framework;

namespace ITCreatings.Ndb.Tests.MySql
{
    [TestFixture]
    public class MySqlDbGatewayTests : DbGatewayTests
    {
        [Test]
        public void FieldSizeTest()
        {
            TestStringLength record = new TestStringLength { Text = "123" };
            Assert.IsTrue(DbTestUtils.SaveTest(record));

            TestStringLength load = gateway.Load<TestStringLength>(record.Id);
            Assert.AreEqual("12", load.Text);
        }

        [Test]
        public void EmptyDateTimeTest()
        {
            var id = Convert.ToUInt64(
                gateway.Accessor.InsertIdentity("Users", "Id",
                                                "Email", "empty@example.com", "Dob", "0000-00-00 00:00:00"));

            var user = gateway.Load<User>(id);
            Assert.IsNotNull(user);

            Assert.AreEqual(DateTime.MinValue, user.Dob);
        }
    }
}
#endif