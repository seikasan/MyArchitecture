using System.Threading;
using Cysharp.Threading.Tasks;

namespace MyArchitecture.Core
{
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
}
