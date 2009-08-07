#if TESTS
using System.Data;
using ITCreatings.Ndb.Accessors.DataReaders;
using NUnit.Framework;

namespace ITCreatings.Ndb.Tests.Accessors
{
    [TestFixture]
    public class FixedWidthTextFilesDataReaderTests
    {
        [Test]
        public void ReadTest()
        {
            using (FixedWidthTextFilesDataReader reader = new FixedWidthTextFilesDataReader(@"Tests\Accessors\FixedWidthTextFileSample.txt"))
            {
                Assert.IsTrue(reader.Read());
                IDataRecord row = reader;
                Assert.AreEqual("1996-12-20 00:00:00.000", row["JustATest_Received_Date"]);
                Assert.AreEqual("1996-12-11 00:00:00.000", row["JustATest_Postmark_Date"]);
                Assert.AreEqual("Ver x", row["JustATest_Version"]);
                Assert.AreEqual("some pretty name", row["Name_Of_Program"]);
                Assert.AreEqual("1", row["Total_No_Of_Children"]);
                Assert.AreEqual("Some status", row["Program_Status"]);
            }
        }
    }
}
#endif