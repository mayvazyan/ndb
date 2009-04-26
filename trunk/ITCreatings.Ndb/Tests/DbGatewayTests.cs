#if DEBUG
using System;
using System.Data;
using ITCreatings.Ndb;
using ITCreatings.Ndb.Query;
using ITCreatings.Ndb.Tests.Data;
using NUnit.Framework;

namespace ITCreatings.Ndb.Tests
{
    [TestFixture]
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
        public void GuidTest()
        {
            if (!gateway.Accessor.IsMySql)
                Assert.Ignore("Supported only by MySQL currently");

            TestGuidRecord record = TestData.CreateTestGuidRecord("test");
            Assert.IsTrue(DbTestUtils.SaveTest(record));

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
        public void GatewayCoreTest()
        {
            var workLog = TestData.CreateWorkLog("Test"); 

            DbTestUtils.SaveTest(workLog);

            workLog.Description = "test2";
            DbTestUtils.UpdateTest(workLog);


            var workLog2 = gateway.Load<TestWorkLog>(workLog.Id);

            AssertWorkLog(workLog, workLog2);

            var workLog3 = new TestWorkLog();
            Assert.IsTrue(gateway.Load(workLog3, "Id", workLog.Id));
            AssertWorkLog(workLog, workLog3);
    
            DbTestUtils.DeleteTest(workLog);

            Assert.IsNull(gateway.Load<TestWorkLog>(workLog.Id));
        }

        private void AssertWorkLog(TestWorkLog workLog, TestWorkLog workLog2)
        {
            Assert.AreEqual(workLog.Id, workLog2.Id);
            Assert.AreEqual(workLog.Description, workLog2.Description);

            Assert.AreEqual(0, DbTestUtils.DiffInSeconds(workLog.Date, workLog2.Date));
            Assert.AreEqual(0, DbTestUtils.DiffInSeconds(workLog.Timestamp, workLog2.Timestamp));

        }

        private static void assert(TestGuidRecord r1, TestGuidRecord r2)
        {
            Assert.AreEqual(r1.Guid, r2.Guid);
            Assert.AreEqual(r1.Title, r2.Title);
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
            var item = new TestWorkLog {UserId = TestData.TestUser.Id, Description = "test"};
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
        public void DbQueryTest()
        {
            User user = TestData.TestUser;
            user.Email = "duser2@example.com";
            gateway.Insert(user);

            user.Email = "duser3@example.com";
            gateway.Insert(user);

            user.Email = "duser4@example.com";
            gateway.Insert(user);

            User[] list = DbQuery<User>
                .Create()
                .Load(gateway);

            Assert.AreEqual(4, list.Length);

            list = DbQuery<User>
                .Create()
                .Contains("Email", "ser3")
                .Load(gateway);

            Assert.AreEqual(1, list.Length);
        }
    }
}
#endif