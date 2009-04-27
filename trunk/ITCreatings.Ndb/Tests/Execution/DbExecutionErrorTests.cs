#if DEBUG
using ITCreatings.Ndb.Exceptions;
using ITCreatings.Ndb.Execution;
using NUnit.Framework;

namespace ITCreatings.Ndb.Tests.Execution
{
    [TestFixture]
    public class DbExecutionErrorTests
    {
        private enum TestEnum
        {
            CustomEntry1,
            CustomEntry2,
        }

        [Test]
        public void EmptyTest()
        {
            var error = DbExecutionError.Empty;
            Assert.IsFalse(error.IsCustomError);
            Assert.IsFalse(error.IsException);
            Assert.IsFalse(error.IsNdbException);
            Assert.AreEqual(DbExecutionError.NO_ERRORS_MESSAGE, error.Message);
        }

        [Test]
        public void ConvertorFromCustomErrorTest()
        {
            DbExecutionError error = TestEnum.CustomEntry1;
            Assert.IsTrue(error.IsCustomError);
            Assert.AreEqual(error.CustomErrorCode, (int) TestEnum.CustomEntry1);
            Assert.AreEqual(DbExecutionError.CUSTOM_ERROR_CODE_MESSAGE + (int) TestEnum.CustomEntry1, error.Message);
            Assert.AreEqual(DbExecutionErrorCode.Custom, error.ErrorCode);

            error = (int) TestEnum.CustomEntry2;
            Assert.AreEqual(error.CustomErrorCode, (int) TestEnum.CustomEntry2);
            Assert.AreEqual(DbExecutionError.CUSTOM_ERROR_CODE_MESSAGE + (int)TestEnum.CustomEntry2, error.Message);
        }


        [Test]
        public void ConvertorFromStringTest()
        {
            DbExecutionError error = "test message";
            Assert.AreEqual("test message", error.Message);
            Assert.IsFalse(error.IsCustomError);
            Assert.IsTrue(error.IsTextMessageError);
        }

        [Test]
        public void ConvertorFromExceptionTest()
        {
            NdbConnectionFailedException exception = new NdbConnectionFailedException("just a test");
            DbExecutionError error = exception;
            Assert.AreEqual(DbExecutionErrorCode.ConnectionFailed, error.ErrorCode);
            Assert.IsFalse(error.IsCustomError);
            Assert.IsTrue(error.IsNdbException);
        }
    }
}
#endif