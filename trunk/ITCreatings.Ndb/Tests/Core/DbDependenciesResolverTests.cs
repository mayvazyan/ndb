#if DEBUG
using System;
using ITCreatings.Ndb.Tests.Data;
using NUnit.Framework;

namespace ITCreatings.Ndb.Tests.Core
{
    [TestFixture]
    public class DbDependenciesResolverTests : DbTestFixture
    {
        [Test]
        public void LoadWithDependencies1LevelTest()
        {
            TestData.AddEvent(EventType.Logon, DateTime.Now);
            ulong userId2 = TestData.AddUser("user2@example.com");

            string query = @"
SELECT * FROM Users;
SELECT * FROM Events;
";
            User[] users = gateway.LoadAndProcessRelations<User>(query, typeof(Event));

            User user1 = users[0];
            User user2 = users[1];

            Assert.AreEqual(user2.Id, userId2);

            Assert.IsNotNull(user1.Events);
            Assert.IsNotNull(user2.Events);

            Assert.AreEqual(1, user1.Events.Length);
            Assert.AreEqual(0, user2.Events.Length);
            Assert.AreEqual(user1.Id, user1.Events[0].UserId);
        }

        [Test]
        public void LoadWithDependencies2LevelTest()
        {
            var oTask = TestData.CreateTask("title #1-1");
            TestData.Assign(oTask);
            var oTask3 = TestData.CreateTask("title #1-2");
            TestData.Assign(oTask3);

            ulong userId2 = TestData.AddUser("user22@example.com");
            var oTask2 = TestData.CreateTask("title #2-1");
            TestData.Assign(oTask2, userId2);

            string query = @"
SELECT * FROM Users;
SELECT * FROM TasksAssignments;
SELECT * FROM Tasks;
";
            User[] users = gateway.LoadAndProcessRelations<User>(query, typeof(TasksAssignment), typeof(Task));

            User user1 = users[0];
            User user2 = users[1];

            Assert.AreEqual(user2.Id, userId2);

            Assert.IsNotNull(user1.TasksAssignments);
            Assert.IsNotNull(user2.TasksAssignments);

            Assert.AreEqual(2, user1.TasksAssignments.Length);
            Assert.AreEqual(1, user2.TasksAssignments.Length);

            Assert.IsNotNull(user1.TasksAssignments[0].Task);
            Assert.IsNotNull(user2.TasksAssignments[0].Task);
        }
    }
}
#endif