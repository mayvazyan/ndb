#if TESTS
using ITCreatings.Ndb.Exceptions;
using ITCreatings.Ndb.Execution;
using ITCreatings.Ndb.Tests.Data;
using NUnit.Framework;

namespace ITCreatings.Ndb.Tests.Execution
{
    [TestFixture]
    public class DbExecutionErrorTests
    {
        [Test]
        public void ConvertorFromCustomExecutionResultCodeTest()
        {
            DbExecutionError<ExecutionResultCode> error = ExecutionResultCode.CustomEntry1;
            Assert.IsTrue(error.IsCustomResultCode);
            Assert.AreEqual(error.ResultCode, ExecutionResultCode.CustomEntry1);
            Assert.AreEqual(DbExecutionErrorMessage.CUSTOM_ERROR_CODE_MESSAGE + ExecutionResultCode.CustomEntry1, error.Message);
            Assert.AreEqual(DbExecutionErrorCode.Custom, error.ErrorCode);

            error = ExecutionResultCode.CustomEntry2;
            Assert.AreEqual(error.ResultCode, ExecutionResultCode.CustomEntry2);
            Assert.AreEqual(DbExecutionErrorMessage.CUSTOM_ERROR_CODE_MESSAGE + ExecutionResultCode.CustomEntry2, error.Message);

            error = ExecutionResultCode.Success;
            Assert.AreEqual(error.ResultCode, ExecutionResultCode.Success);
            Assert.AreEqual(DbExecutionErrorMessage.NO_ERRORS_MESSAGE, error.Message);
        }


        [Test]
        public void ConvertorFromStringTest()
        {
            DbExecutionError<int> error = "test message";
            Assert.AreEqual("test message", error.Message);
            Assert.IsFalse(error.IsCustomResultCode);
            Assert.IsTrue(error.IsTextMessageError);
        }

        [Test]
        public void ConvertorFromExceptionTest()
        {
            NdbConnectionFailedException exception = new NdbConnectionFailedException("just a test");
            DbExecutionError<int> error = exception;
            Assert.AreEqual(DbExecutionErrorCode.ConnectionFailed, error.ErrorCode);
            Assert.IsFalse(error.IsCustomResultCode);
            Assert.IsTrue(error.IsNdbException);
        }
    }
}
#endif