using System;
using ITCreatings.Ndb.Tests.Data;
using NUnit.Framework;

#if DEBUG
namespace ITCreatings.Ndb.Tests.MySql
{
    [TestFixture]
    public class MySqlDbGatewayTests : DbGatewayTests
    {
        [Test]
        public void EmpyDataFix()
        {
            var id = Convert.ToUInt64(
                gateway.Accessor.InsertIdentity("Users", "Id",
                "Email", "empty@example.com", "Dob", "0000-00-00 00:00:00"));

            var user = gateway.Load<User>(id);
            Assert.IsNotNull(user);

            Assert.AreEqual(DateTime.MinValue, user.Dob);
        }
    }

    [TestFixture]
    public class MySqlDbStructureGatewayTests : DbStructureGatewayTests
    {
    }
}
#endif