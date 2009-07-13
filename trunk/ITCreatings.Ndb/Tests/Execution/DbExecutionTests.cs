#if TESTS
using System;
using ITCreatings.Ndb.Execution;
using ITCreatings.Ndb.Tests.Data;
using NUnit.Framework;

namespace ITCreatings.Ndb.Tests.Execution
{
    [TestFixture]
    public class DbExecutionTests
    {
        private TestData TestData;

        [SetUp]
        public void SetUp()
        {
            TestData = new TestData(null);
        }

        [Test]
        public void WrapperTest()
        {
        }
        [Test]
        public void CustomResultCodeErrorTest()
        {
            User user = TestData.TestUser;

            Assert.AreEqual(0, user.Id);

            user.Password = "123";
            
            var executor = DbExecution<User, ExecutionResultCode>.Create()
                .Execute(user, LoginUser);

            Assert.IsTrue(executor.IsError);
            Assert.AreEqual(ExecutionResultCode.InvalidPasswordLength, (ExecutionResultCode) executor.Error);
            Assert.AreEqual(
                DbExecutionErrorMessage.CUSTOM_ERROR_CODE_MESSAGE + ExecutionResultCode.InvalidPasswordLength,
                executor.Error.Message);
            Assert.AreEqual(0, user.Id);


            user.Password = "123456789";
            executor = DbExecution<User, ExecutionResultCode>.Create()
                .Execute(user, LoginUser);

            Assert.IsFalse(executor.IsError);
            Assert.IsNotNull(executor.Error);
            Assert.AreEqual(DbExecutionErrorMessage.NO_ERRORS_MESSAGE, executor.Error.Message);

            Assert.AreEqual(7, executor.Result.Id);
        }

        [Test]
        public void StringMessageErrorTest()
        {
            User user = TestData.TestUser;
            string message1 = "User Id should be more than zero";
            string message2 = "Password Is Empty";

            var executor = DbExecution<User, ExecutionResultCode>.Create()
                .IsZero(user.Id, message1)
                .IsFalse(string.IsNullOrEmpty(user.Password), message2)
                .Execute(user, LoginUser);

            Assert.IsTrue(executor.IsError);
            Assert.AreEqual(message2, executor.Error.Message);
        }

        [Test]
        public void AnonymousDelegateTest()
        {
            User user = TestData.TestUser;
            Assert.AreEqual(0, user.Id);

            var executor = DbExecution<User, ExecutionResultCode>.Create()
                .Execute(exec => exec.Error = "Test");

            Assert.IsTrue(executor.IsError);
            Assert.AreEqual("Test", executor.Error.Message);

            executor = DbExecution<User, ExecutionResultCode>.Create()
               .Execute(exec =>
                            {
                                user.Id = 5;
                                exec.Result = user;
                            });

            Assert.IsFalse(executor.IsError);
            Assert.AreEqual(5, executor.Result.Id);
        }

        private static void LoginUser(object data, IDbExecution<User, ExecutionResultCode> execution)
        {
            var user = (User)data;

            if (user.Password.Length < 7)
                execution.Error = ExecutionResultCode.InvalidPasswordLength;
            else
            {
                user.Id = 7;
                execution.Result = user;
            }
        }

        #region ExecutionResultCodeTest

        const string EXCEPTION = "just a test";

        [Test]
        public void PossibleExecutionResultCodeTest()
        {
            var execution = DbExecution<User, ExecutionResultCode>.Create()
                .SetPossibleResultCode(ExecutionResultCode.UnableLoadData)
                .Execute(exec => { throw new Exception(EXCEPTION); });//emulate exception during data load 

            Assert.IsTrue(execution.IsError);
            Assert.IsTrue(execution.Error.IsException);
            Assert.IsTrue(execution.Error.IsCustomResultCode);
            Assert.AreEqual(ExecutionResultCode.UnableLoadData, execution.Error.ResultCode);
            
            Assert.AreEqual(EXCEPTION, execution.Error.Exception.Message);
        }

        #endregion
    }
}
#endif