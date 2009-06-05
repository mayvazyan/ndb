#if DEBUG
using ITCreatings.Ndb.Import;
using NUnit.Framework;

namespace ITCreatings.Ndb.Tests.Import
{
    [TestFixture]
    public class GenericTests
    {
        [Test]
        public void RemoveExtraSpacesTest()
        {
            Assert.AreEqual("John Doe", DbImportUtils.RemoveExtraSpaces("John  Doe "));
        }
    }
}
#endif