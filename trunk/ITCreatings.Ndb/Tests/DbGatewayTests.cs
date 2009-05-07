#if DEBUG
using System;
using System.Collections.Generic;
using ITCreatings.Ndb;
using ITCreatings.Ndb.Exceptions;
using ITCreatings.Ndb.Query;
using ITCreatings.Ndb.Tests.Data;
using NUnit.Framework;

namespace ITCreatings.Ndb.Tests
{
    public class DbGatewayTests : DbTestFixture
    {
        internal DbGatewayTests(DbAccessor accessor) : base(accessor)
        {
        }

        public DbGatewayTests()
        {
        }

        [Test]
        public void ConnectionTest()
        {
            Assert.IsTrue(gateway.Accessor.CanConnect);
        }

        [Test]
        public void BinaryTest()
        {
            const int value = 777;

            BinaryDataRecord record = new BinaryDataRecord {Data = BitConverter.GetBytes(value) };
            Assert.IsTrue(DbTestUtils.SaveTest(record));

            BinaryDataRecord record2 = gateway.Load<BinaryDataRecord>(record.Id);
            Assert.AreEqual(record.Data, record2.Data);
            Assert.AreEqual(value, BitConverter.ToInt32(record2.Data, 0));
        }

        [Test]
        public void ConvertBinaryToIntTest()
        {
            var record = new BinaryDataRecord2 { Data = 77 };

            Assert.IsTrue(DbTestUtils.SaveTest(record));

            var record2 = gateway.Load<BinaryDataRecord2>(record.Id);
            Assert.AreEqual(record.Data, record2.Data);
        }

        [Test]
        public void ConvertBinaryToStringTest()
        {
            var record = new BinaryDataRecord3 { Data = "77" };

            Assert.IsTrue(DbTestUtils.SaveTest(record));

            var record2 = gateway.Load<BinaryDataRecord3>(record.Id);
            Assert.AreEqual(record.Data, record2.Data);
        }

        private void CheckIsGuidSupports()
        {
            if (!IsGuidSupported)
                Assert.Ignore("GUID Not Supported by {0} currently", gateway.Accessor);
        }

        [Test]
        public void GuidTest()
        {
            CheckIsGuidSupports();

            TestGuidRecord record = TestData.CreateTestGuidRecord("test");
            Assert.IsTrue(DbTestUtils.SaveTest(record));

            var recordLoaded = gateway.Load<TestGuidRecord>(record.Guid);
            assert(record, recordLoaded);

            record.Title = "test2";
            Assert.IsTrue(DbTestUtils.UpdateTest(record));


            var workLog2 = gateway.Load<TestGuidRecord>(record.Guid);
            
            Assert.IsNotNull(workLog2);
            assert(record, workLog2);

            var workLog3 = new TestGuidRecord();
            Assert.IsTrue(gateway.Load(workLog3, "Guid", record.Guid));
            assert(record, workLog3);

            DbTestUtils.DeleteTest(record);

            Assert.IsNull(gateway.Load<TestGuidRecord>(record.Guid));
        }

        [Test]
        public void InsertGuidTest()
        {
            CheckIsGuidSupports();

            TestGuidRecord record = TestData.CreateTestGuidRecord("test");
            ulong count = gateway.LoadCount(typeof(TestGuidRecord));
            gateway.Insert(record);
            ulong count2 = gateway.LoadCount(typeof(TestGuidRecord));
            Assert.AreEqual(count + 1, count2);

            gateway.Delete(record);

            count2 = gateway.LoadCount(typeof(TestGuidRecord));
            Assert.AreEqual(count, count2);

        }

        [Test]
        public void EmptyGuidTest()
        {
            CheckIsGuidSupports();

            TestGuidRecord record2 = TestData.CreateTestGuidRecord("test");
            record2.TestGuidField = Guid.Empty;
            Assert.IsTrue(DbTestUtils.SaveTest(record2));

            var record2Loaded = gateway.Load<TestGuidRecord>(record2.Guid);
            assert(record2, record2Loaded);
        }

        [Test]
        public void GatewayCoreTest()
        {
            var workLog = TestData.CreateWorkLog("Test"); 

            DbTestUtils.SaveTest(workLog);

            workLog.Description = "test2";
            DbTestUtils.UpdateTest(workLog);


            var workLog2 = gateway.Load<WorkLog>(workLog.Id);

            AssertWorkLog(workLog, workLog2);

            var workLog3 = new WorkLog();
            Assert.IsTrue(gateway.Load(workLog3, "Id", workLog.Id));
            AssertWorkLog(workLog, workLog3);
    
            DbTestUtils.DeleteTest(workLog);

            Assert.IsNull(gateway.Load<WorkLog>(workLog.Id));
        }

        private void AssertWorkLog(WorkLog workLog, WorkLog workLog2)
        {
            Assert.AreEqual(workLog.Id, workLog2.Id);
            Assert.AreEqual(workLog.Description, workLog2.Description);

            Assert.AreEqual(0, DbTestUtils.DiffInSeconds(workLog.Date, workLog2.Date));
            Assert.AreEqual(0, DbTestUtils.DiffInSeconds(workLog.Timestamp, workLog2.Timestamp));

        }

        private static void assert(TestGuidRecord r1, TestGuidRecord r2)
        {
            Assert.IsNotNull(r1);
            Assert.IsNotNull(r2);
            Assert.AreEqual(r1.Guid, r2.Guid);
            Assert.AreEqual(r1.Title, r2.Title);
            Assert.AreEqual(r1.TestGuidField, r2.TestGuidField);
        }


        /// <summary>
        /// native to sql engine cascade delete should work
        /// </summary>
        [Test]
        public void CascadeDeleteTest()
        {
            ulong count = gateway.LoadCount(typeof(TasksAssignment));

            var oTask = TestData.CreateTask("title #1");
            TestData.Assign(oTask);
            
            ulong count2 = gateway.LoadCount(typeof(TasksAssignment));
            Assert.AreEqual(count + 1, count2);

            gateway.Delete(oTask);
            ulong count3 = gateway.LoadCount(typeof (TasksAssignment));
            Assert.AreEqual(count, count3);
        }

        [Test]
        public void LoadRelatedTest()
        {
            var oTask = TestData.CreateTask("title #1");
            TestData.Assign(oTask);
            
            Task [] task = gateway.LoadAssociated<Task, TasksAssignment>(TestData.TestUser);

            Assert.AreEqual(1, task.Length);
            Assert.AreEqual(oTask.Title, task[0].Title);
        }

        [Test]
        public void LoadParentTest()
        {
            var item = new WorkLog {UserId = TestData.TestUser.Id, Description = "test"};
            gateway.Save(item);

            User testUser = gateway.LoadParent<User>(item);
            Assert.IsNotNull(testUser);
            Assert.AreEqual(testUser.Id, TestData.TestUser.Id);
            Assert.AreEqual(testUser.FullName, TestData.TestUser.FullName);
        }

        [Test]
        public void LoadChildsTest()
        {
            TestData.AddEvent(EventType.Logon, DateTime.Now);

            Event[] events = gateway.LoadChilds<Event>(TestData.TestUser);
            Assert.AreEqual(1, events.Length);
            Assert.AreEqual(EventType.Logon, events[0].EventTypeId);

            TestData.AddEvent(EventType.ActivityLog, DateTime.Now);
            TestData.AddEvent(EventType.Logon, DateTime.Now);

            events = gateway.LoadChilds<Event>(TestData.TestUser);
            Assert.AreEqual(3, events.Length);

            events = gateway.LoadChilds<Event>(TestData.TestUser, "EventTypeId", EventType.ActivityLog);
            Assert.AreEqual(1, events.Length);
        }

        [Test]
        public void UpdateTest()
        {
            User user = TestData.TestUser;
            user.Email = "test2@example.com";
            gateway.Insert(user);

            Assert.AreEqual(2, gateway.LoadList<User>("LastName", "Doe").Length);

            int affected = gateway.Update(typeof(User),
                new object [] { "LastName", "Not a Doe" }, 
                "Email", user.Email);

            Assert.AreEqual(1, affected);

            Assert.AreEqual(1, gateway.LoadList<User>("LastName", "Doe").Length);
        }

        [Test]
        public void LoadLimitedTest()
        {
            User user = TestData.TestUser;
            user.Email = "user2@example.com";
            gateway.Insert(user);

            user.Email = "user3@example.com";
            gateway.Insert(user);

            user.Email = "user4@example.com";
            gateway.Insert(user);


            User[] users = gateway.LoadListLimited<User>(2, 2);
            Assert.AreEqual(2, users.Length);
            Assert.AreEqual("user3@example.com", users[0].Email);
            Assert.AreEqual("user4@example.com", users[1].Email);

            users = gateway.LoadListLimited<User>(3, 1);
            Assert.AreEqual(3, users.Length);
            Assert.AreEqual("user2@example.com", users[0].Email);
            Assert.AreEqual("user3@example.com", users[1].Email);
            Assert.AreEqual("user4@example.com", users[2].Email);
        }

        [Test]
        [ExpectedException(typeof(NdbInvalidColumnNameException))]
        public void LoadListFilterExpressionExceptionTest()
        {
            var expression = new DbColumnFilterExpression(DbExpressionType.Equal, "Em?ail", "some email");
            var expressions = new List<DbFilterExpression> {expression};

            User[] users = gateway.Select(expressions).Load<User>();
        }

        [Test]
        public void LoadListFilterExpressionTest()
        {
            var expression = new DbColumnFilterExpression(
                DbExpressionType.Equal, "Email", TestData.TestUser.Email);
            var expressions = new List<DbFilterExpression>();
            expressions.Add(expression);

            User[] list = gateway.Select(expressions).Load<User>();
            Assert.AreEqual(1, list.Length);

            expression.Value = "other@example.com";
            list = gateway.Select(expressions).Load<User>();
            Assert.AreEqual(0, list.Length);
        }

        [Test]
        public void LoadResultFilterExpressionTest()
        {
            if (!gateway.Accessor.IsMySql)
                Assert.Ignore("LoadResult currently supported only by mysql");

            var expression = new DbColumnFilterExpression(
                DbExpressionType.Equal, "Email", TestData.TestUser.Email);
            var expressions = new List<DbFilterExpression>();
            expressions.Add(expression);

            DbQueryResult<User> result = gateway.Select(expressions).LoadResult<User>(true);
            Assert.AreEqual(1, result.Records.Length);
            Assert.AreEqual(1, result.TotalRecordsCount);

            Assert.AreEqual(1, gateway.Select(expressions).LoadCount<User>());

            result = gateway.Select(expressions).LoadResult<User>(false);
            Assert.AreEqual(1, result.Records.Length);
            Assert.AreEqual(0, result.TotalRecordsCount);

            expression.Value = "other@example.com";
            result = gateway.Select(expressions).LoadResult<User>(true);
            Assert.AreEqual(0, result.Records.Length);
            Assert.AreEqual(0, result.TotalRecordsCount);
            Assert.AreEqual(0, gateway.Select(expressions).LoadCount<User>());
        }
        
        #region DbQuery

        [Test]
        public void DbQueryFiltersTest()
        {
            User user = TestData.TestUser;
            user.Email = "duser2@example.com";
            gateway.Insert(user);

            user.Email = "duser'3@example.com";
            gateway.Insert(user);

            user.Email = "duser4@example.com";
            user.LastName = null;
            gateway.Insert(user);

            User[] usersList = DbQuery
                .Create(gateway)
                .Load<User>();

            Assert.AreEqual(4, usersList.Length);

            var list = DbQuery.Create(gateway)
                .Contains("Email", "ser'3")
                .Load<User>();

            Assert.AreEqual(1, list.Length);

            list = DbQuery.Create(gateway)
                .Contains("Email", "4@e")
                .StartsWith("Email", "duser4@")
                .EndsWith("Email", "4@example.com")
                .IsNotNull("Email")
                .IsNull("LastName")
                .Greater("Id", 0)
                .Less("Id", 100)
                .Load<User>();

            Assert.AreEqual(1, list.Length);
        }

        [Test]
        public void DbQueryOrderByTest()
        {
            User user = TestData.TestUser;
            user.Email = "teat@example.com";
            gateway.Insert(user);


            var list = DbQuery.Create(gateway)
                .OrderBy("Email")
                .Load<User>();

            Assert.AreEqual(2, list.Length);
            Assert.AreEqual(user.Email, list[0].Email);

            list = DbQuery.Create(gateway)
                .OrderBy("Email", DbSortingDirection.Desc)
                .Load<User>();

            Assert.AreEqual(2, list.Length);
            Assert.AreEqual(user.Email, list[1].Email);
        }

        [Test]
        public void DbQueryLimitest()
        {
            User user = TestData.TestUser;
            User user2 = TestData.TestUser2;
            
            gateway.Insert(user2);

            var list = DbQuery
                .Create(gateway)
                .OrderBy("Email")
                .Limit(1)
                .Load<User>();

            Assert.AreEqual(1, list.Length);
            Assert.AreEqual(user.Email, list[0].Email);

            list = gateway.Select()
                .OrderBy("Email")
                .Limit(1)
                .Offset(1)
                .Load<User>();

            Assert.AreEqual(1, list.Length);
            Assert.AreEqual(user2.Email, list[0].Email);
        }

        #endregion

        [Test]
        public void Loadtables()
        {
            string[] tables = gateway.Accessor.LoadTables();
            Assert.Less(0, tables.Length);
        }

    }
}
#endif