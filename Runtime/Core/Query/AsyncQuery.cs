using System.Threading;
using Cysharp.Threading.Tasks;

namespace MyArchitecture.Core
{
    // 0
    public abstract class AsyncQuery<TResult> :
        QueryBase,
        IAsyncQuery<TResult>
    {
        public UniTask<TResult> ExecuteAsync(
            CancellationToken cancellationToken)
        {
            return OnExecuteAsync(cancellationToken);
        }

        protected abstract UniTask<TResult> OnExecuteAsync(
            CancellationToken cancellationToken);
    }

    // 1
    public abstract class AsyncQuery<TArg, TResult> :
        QueryBase,
        IAsyncQuery<TArg, TResult>
    {
        public UniTask<TResult> ExecuteAsync(
            TArg arg,
            CancellationToken cancellationToken)
        {
            return OnExecuteAsync(arg, cancellationToken);
        }

        protected abstract UniTask<TResult> OnExecuteAsync(
            TArg arg,
            CancellationToken cancellationToken);
    }

    // 2
    public abstract class AsyncQuery<TArg1, TArg2, TResult> :
        QueryBase,
        IAsyncQuery<TArg1, TArg2, TResult>
    {
        public UniTask<TResult> ExecuteAsync(
            TArg1 arg1,
            TArg2 arg2,
            CancellationToken cancellationToken)
        {
            return OnExecuteAsync(arg1, arg2, cancellationToken);
        }

        protected abstract UniTask<TResult> OnExecuteAsync(
            TArg1 arg1,
            TArg2 arg2,
            CancellationToken cancellationToken);
    }

    // 3
    public abstract class AsyncQuery<TArg1, TArg2, TArg3, TResult> :
        QueryBase,
        IAsyncQuery<TArg1, TArg2, TArg3, TResult>
    {
        public UniTask<TResult> ExecuteAsync(
            TArg1 arg1,
            TArg2 arg2,
            TArg3 arg3,
            CancellationToken cancellationToken)
        {
            return OnExecuteAsync(arg1, arg2, arg3, cancellationToken);
        }

        protected abstract UniTask<TResult> OnExecuteAsync(
            TArg1 arg1,
            TArg2 arg2,
            TArg3 arg3,
            CancellationToken cancellationToken);
    }
}
