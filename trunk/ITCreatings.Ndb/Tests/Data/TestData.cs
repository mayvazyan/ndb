#if TESTS
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

        public User TestUser2 = new User
        {
            Email = "user2@example.com",
            LastName = "Doe",
            FirstName = "John",
            RoleId = RolesManager.Ids.Developer
        };

        public User GetTestUser()
        {
            return GetTestUser("test@example.com");
        }

        public User GetTestUser(string email)
        {
                return new User
                {
                    Email = email,
                    LastName = "Doe",
                    FirstName = "John",
                    RoleId = RolesManager.Ids.Developer
                };
        }

        public User TestUser = new User
                                   {
                                       Email = "test@example.com",
                                       LastName = "Doe",
                                       FirstName = "John",
                                       RoleId = RolesManager.Ids.Developer
                                   };

        public Event TestEvent = new Event
        {
            EventTypeId = EventType.ActivityLog,
            Timestamp = DateTime.Now,
        };

        public Event TestEvent2 = new Event
        {
            EventTypeId = EventType.IdleEnd,
            Timestamp = DateTime.Now,
        };

        public Event TestEvent3 = new Event
        {
            EventTypeId = EventType.IdleEnd,
            Timestamp = DateTime.Now,
        };

        public const string sqLyogName = "SQLyog Enterprise";
        public const string sqLyogTitle = sqLyogName + " - MySQL GUI";
        public const string VisualStudioName = "Microsoft Visual Studio";
        public const string VisualStudioTitle = "WTA - " + VisualStudioName;

        public long CreateUser()
        {
            return AddUser(TestUser);
        }

        public long AddUser(string email)
        {
            User user = TestUser;
            user.Email = email;
            return AddUser(user);
        }

        private long AddUser(User user)
        {
            gateway.Insert(user);

            return user.Id;
        }

        public void Clean()
        {
            if (TestUser != null)
                gateway.Delete(TestUser);

            gateway.Delete(typeof(WorkLog), "UserId", TestUser.Id);

            gateway.Delete(typeof(Event), "UserId", TestUser.Id);
        }

        public void AddEvent(EventType eventType, DateTime timestamp)
        {
            Event foo = new Event {UserId = TestUser.Id, EventTypeId = eventType, Timestamp = timestamp};
            gateway.Save(foo);
        }

        public void AddWorkLog(int minutes, DateTime day)
        {
            WorkLog workLog = new WorkLog
                                  {
                                      SpentMinutes = minutes,
                                      Date = day,
                                      Timestamp = DateTime.Now,
                                      UserId = TestUser.Id
                                  };

            gateway.Save(workLog);
        }

        public Task CreateTask(string title)
        {
            var oTask = new Task {Title = title};

            gateway.Save(oTask);

            return oTask;
        }

        public void Assign(Task task)
        {
            Assign(task, TestUser.Id);
        }

        public void Assign(Task task, long userId)
        {
            var assignment = new TasksAssignment {UserId = userId, TaskId = task.Id};
            gateway.Save(assignment);
        }

        /*public static void Assign2(Task task)
        {
            var assignment = new TasksAssignment2();
            assignment.UserId = User.Id;
            assignment.TaskId = task.Id;
            gateway.Save(assignment);
        }*/

        public WorkLog CreateWorkLog(string title)
        {
            return new WorkLog
                       {
                           Date = DateTime.Now,
                           Description = title,
                           Timestamp = DateTime.Now,
                           UserId = TestUser.Id
                       };

        }

        public TestGuidRecord CreateTestGuidRecord(string title)
        {
            return new TestGuidRecord {Title = title, TestGuidField = Guid.NewGuid()};
        }
    }
}
#endif