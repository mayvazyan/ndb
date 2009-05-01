#if DEBUG
using ITCreatings.Ndb.Utils;
using NUnit.Framework;

namespace ITCreatings.Ndb.Tests.Utils
{
    [TestFixture]
    public class DbValidatorTests
    {
        [Test]
        public void IsValidColumnNameTest()
        {
            var validated = DbValidator.IsValidColumnName("TestColumnName");
            Assert.AreEqual(true, validated);

            validated = DbValidator.IsValidColumnName("Test#C%^&*\r\n\tolumn Name");
            Assert.AreEqual(false, validated);
        }
    }
}
#endif