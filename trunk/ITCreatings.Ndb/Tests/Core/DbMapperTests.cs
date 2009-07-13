#if TESTS
using ITCreatings.Ndb.Core;
using ITCreatings.Ndb.Tests.Data;
using NUnit.Framework;

namespace ITCreatings.Ndb.Tests.Core
{
    [TestFixture]
    public class DbRecordsMapperTests
    {
        private TestData TestData = new TestData(DbAccessor.Instance);

        [Test]
        public void MapTest()
        {
            User user1 = TestData.TestUser;
            User user2 = TestData.TestUser2;
            user1.Id = 1;
            user2.Id = 2;

            Event event1 = TestData.TestEvent;
            Event event2 = TestData.TestEvent2;
            Event event3 = TestData.TestEvent3;
            event1.UserId = user1.Id;
            event2.UserId = user2.Id;
            event3.UserId = user2.Id;
            event1.Id = 1;
            event2.Id = 2;
            event3.Id = 3;

            DbRecordsMapper.Map(new [] {user1, user2}, new [] {event1, event2, event3});

            Assert.AreEqual(1, user1.Events.Length);
            Assert.AreEqual(2, user2.Events.Length);

            Assert.AreEqual(user1.Events[0].Id, event1.Id);
            Assert.AreEqual(user2.Events[1].Id, event2.Id);
            Assert.AreEqual(user2.Events[0].Id, event3.Id);
        }
    }
}
#endif