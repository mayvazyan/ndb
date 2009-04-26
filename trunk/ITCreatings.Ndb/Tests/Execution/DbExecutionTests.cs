#if DEBUG
using ITCreatings.Ndb.Execution;
using ITCreatings.Ndb.Tests.Data;
using NUnit.Framework;

namespace ITCreatings.Ndb.Tests.Execution
{
    [TestFixture]
    public class DbExecutionTests : DbTestFixture
    {
        [Test]
        public void ValidateTest()
        {
            User user = TestData.TestUser;
            string message1 = "User Id should be more than zero";
            string message2 = "Password Is Empty";

            Assert.AreEqual(1, user.Id);

            var executor = DbExecution<User>.Create()
                .Execute(user, LoginUser);

            Assert.IsFalse(executor.IsError);
            Assert.AreEqual(7, executor.Result.Id);

            executor = DbExecution<User>.Create()
                .Validate(user.Id == 0, message1)
                .Validate(string.IsNullOrEmpty(user.Password), message2)
                .Execute(user, LoginUser);

            Assert.IsTrue(executor.IsError);
            Assert.AreEqual(message2, executor.Error.Message);
        }

        [Test]
        public void AnonymousDelegateTest()
        {
            User user = TestData.TestUser;
            Assert.AreEqual(1, user.Id);

            var executor = DbExecution<User>.Create()
                .Execute(delegate (IExecution<User> execution) { execution.Error = "Test"; });

            Assert.IsTrue(executor.IsError);
            Assert.AreEqual("Test", executor.Error.Message);

            executor = DbExecution<User>.Create()
               .Execute(delegate(IExecution<User> execution)
                            {
                                user.Id = 5;
                                execution.Result = user;
                            });

            Assert.IsFalse(executor.IsError);
            Assert.AreEqual(5, executor.Result.Id);
        }

        private User LoginUser(object data, IExecution<User> execution)
        {
            User user = (User)data;
            user.Id = 7;
            return user;
        }
    }
}
#endif