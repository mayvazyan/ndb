#if TESTS
using ITCreatings.Ndb.Execution;
using ITCreatings.Ndb.Tests.Data;

namespace ITCreatings.Ndb.Tests.Execution
{
    public interface IExecutionFlow<TResult> : IDbExecution<TResult, ExecutionResultCode>
    {
    }

    public class ExecutionFlow<TResult> : DbExecution<TResult, ExecutionResultCode>
    {
//        public DbExecution<TResult, ExecutionResultCode> Execute(Action<IExecutionFlow<TResult>> action)
//        {
//            return Execute()
//        }
    }
}
#endif