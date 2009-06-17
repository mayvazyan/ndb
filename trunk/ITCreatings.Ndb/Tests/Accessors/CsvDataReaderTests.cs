#if DEBUG
using ITCreatings.Ndb.Accessors.DataReaders;
using ITCreatings.Ndb.Attributes;
using NUnit.Framework;

namespace ITCreatings.Ndb.Tests.Accessors
{
    public class CsvUser
    {
        [DbField] 
        public int Id;

        [DbField]
        public string FirstName;

        [DbField]
        public string LastName;

        [DbField]
        public string Note;
    }

    [TestFixture]
    public class CsvDataReaderTests
    {
        private static CsvDataReader GetReader(string name)
        {
            return new CsvDataReader(string.Format(@"Tests\Accessors\{0}Csv.txt", name));
        }

        [Test]
        public void SingleLineTest()
        {
            using (CsvDataReader reader = GetReader("SingleLine"))
            {
                CsvUser[] users = reader.LoadList<CsvUser>();

                Assert.AreEqual(2, users.Length);
                CsvUser user1 = users[0];
                Assert.AreEqual(1, user1.Id);
                Assert.AreEqual("John", user1.FirstName);
                Assert.AreEqual("Doe", user1.LastName);
                Assert.AreEqual("note #1", user1.Note);

                CsvUser user2 = users[1];
                Assert.AreEqual(2, user2.Id);
                Assert.AreEqual("Mike", user2.FirstName);
                Assert.AreEqual("NotDoe", user2.LastName);
                Assert.AreEqual("note #2 ", user2.Note);
            }
        }

        [Test]
        public void MultiLineTest()
        {
            using (CsvDataReader reader = GetReader("MultiLine"))
            {
                reader.AttemptToFixMultiline = true;

                CsvUser[] users = reader.LoadList<CsvUser>();

                Assert.AreEqual(2, users.Length);
                CsvUser user1 = users[0];
                Assert.AreEqual(1, user1.Id);
                Assert.AreEqual("John", user1.FirstName);
                Assert.AreEqual("Doe", user1.LastName);
                Assert.AreEqual(@"Multiline note line 1
Multiline note line 2", user1.Note);

                CsvUser user2 = users[1];
                Assert.AreEqual(2, user2.Id);
                Assert.AreEqual("Mike", user2.FirstName);
                Assert.AreEqual("NotDoe", user2.LastName);
                Assert.AreEqual("some note", user2.Note);
            }
        }
    }
}
#endif