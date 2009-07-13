#if TESTS
using System.Data;
using ITCreatings.Ndb.Query;
using ITCreatings.Ndb.Query.Filters;
using ITCreatings.Ndb.Tests.Data;
using NUnit.Framework;

namespace ITCreatings.Ndb.Tests.Query
{
    [TestFixture]
    [Ignore("Not completed yet")]
    public class DbQueryTests
    {
        [Test]
        public void GetFiltersSqlTest()
        {
            TestData TestData = new TestData(DbAccessor.Instance);

            var expression = new DbColumnFilterExpression(
                DbExpressionType.Equal, "Email", TestData.TestUser.Email);

            var expression2 = new DbColumnFilterExpression(
                DbExpressionType.Equal, "Email", "user2@example.com");

            DbFilterGroup or = new DbOrFilterGroup(expression, expression2);

            var gateway = new DbGateway(DbAccessor.Instance);
        
            //TODO: implement below approach
//            DataTable table = DbQuery.Create(gateway, "SELECT * FROM Users", or).LoadDataTable();
        }
    }
}
#endif