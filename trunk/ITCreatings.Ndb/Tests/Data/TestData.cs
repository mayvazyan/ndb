#if DEBUG
using System;
using ITCreatings.Ndb;
using ITCreatings.Ndb.Tests.Data;

namespace ITCreatings.Ndb.Tests.Data
{
    public class TestData
    {
        public DbGateway gateway { get; private set; }

        public TestData(DbAccessor accessor)
        {
            gateway = new DbGateway(accessor);
        }

        public void SetUp()
        {
        }

        public void TearDown()
        {
        }

        public TestUser TestUser = new TestUser
                       {
                           Email = "test@example.com",
                           LastName = "Doe",
                           FirstName = "John",
                           RoleId = RolesManager.Ids.Developer
                       };

        public const string sqLyogName = "SQLyog Enterprise";
        public const string sqLyogTitle = sqLyogName + " - MySQL GUI";
        public const string VisualStudioName = "Microsoft Visual Studio";
        public const string VisualStudioTitle = "WTA - " + VisualStudioName;

        public ulong CreateUser()
        {
            gateway.Insert(TestUser);

            return TestUser.Id;
        }

        public void Clean()
        {
            if (TestUser != null)
                gateway.Delete(TestUser);

            gateway.Delete(typeof(TestWorkLog), "UserId", TestUser.Id);

            gateway.Delete(typeof(Event), "UserId", TestUser.Id);
        }

        public void AddEvent(EventType eventType, DateTime timestamp)
        {
            Event foo = new Event() {UserId = TestUser.Id, EventTypeId = eventType, Timestamp = timestamp};
            gateway.Save(foo);
        }

        public void AddWorkLog(uint minutes, DateTime day)
        {
            TestWorkLog testWorkLog = new TestWorkLog
                                  {
                                      SpentMinutes = minutes,
                                      Date = day,
                                      Timestamp = DateTime.Now,
                                      UserId = TestUser.Id
                                  };

            gateway.Save(testWorkLog);
        }

        public Task CreateTask(string title)
        {
            var oTask = new Task {Title = title};

            gateway.Save(oTask);

            return oTask;
        }

        public void Assign(Task task)
        {
            var assignment = new TasksAssignment();
            assignment.UserId = TestUser.Id;
            assignment.TaskId = task.Id;
            gateway.Save(assignment);
        }

        /*public static void Assign2(Task task)
        {
            var assignment = new TasksAssignment2();
            assignment.UserId = TestUser.Id;
            assignment.TaskId = task.Id;
            gateway.Save(assignment);
        }*/

        public TestWorkLog CreateWorkLog(string title)
        {
            return new TestWorkLog
                       {
                           Date = DateTime.Now,
                           Description = title,
                           Timestamp = DateTime.Now,
                           UserId = TestUser.Id
                       };

        }

        public TestGuidRecord CreateTestGuidRecord(string title)
        {
            return new TestGuidRecord {Title = title};
        }
    }
}
#endif