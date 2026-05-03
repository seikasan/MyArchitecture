using System.Threading;
using Cysharp.Threading.Tasks;

namespace MyArchitecture.Core
{
    public abstract class AsyncResultCommand<TResult> :
        CommandBase,
        IAsyncResultCommand<TResult>
        where TResult : ICommandResult
    {
        public UniTask<TResult> ExecuteAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return OnExecuteAsync(cancellationToken);
        }

        protected abstract UniTask<TResult> OnExecuteAsync(
            CancellationToken cancellationToken);
    }

    public abstract class AsyncResultCommand<TArg, TResult> :
        CommandBase,
        IAsyncResultCommand<TArg, TResult>
        where TResult : ICommandResult
    {
        public UniTask<TResult> ExecuteAsync(
            TArg arg,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return OnExecuteAsync(arg, cancellationToken);
        }

        protected abstract UniTask<TResult> OnExecuteAsync(
            TArg arg,
            CancellationToken cancellationToken);
    }
}