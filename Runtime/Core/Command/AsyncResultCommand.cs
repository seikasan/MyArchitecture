using System.Threading;
using Cysharp.Threading.Tasks;

namespace MyArchitecture.Core
{
    // 0
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

    // 1
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

    // 2
    public abstract class AsyncResultCommand<TArg1, TArg2, TResult> :
        CommandBase,
        IAsyncResultCommand<TArg1, TArg2, TResult>
        where TResult : ICommandResult
    {
        public UniTask<TResult> ExecuteAsync(
            TArg1 arg1,
            TArg2 arg2,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return OnExecuteAsync(arg1, arg2, cancellationToken);
        }

        protected abstract UniTask<TResult> OnExecuteAsync(
            TArg1 arg1,
            TArg2 arg2,
            CancellationToken cancellationToken);
    }

    // 3
    public abstract class AsyncResultCommand<TArg1, TArg2, TArg3, TResult> :
        CommandBase,
        IAsyncResultCommand<TArg1, TArg2, TArg3, TResult>
        where TResult : ICommandResult
    {
        public UniTask<TResult> ExecuteAsync(
            TArg1 arg1,
            TArg2 arg2,
            TArg3 arg3,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return OnExecuteAsync(arg1, arg2, arg3, cancellationToken);
        }

        protected abstract UniTask<TResult> OnExecuteAsync(
            TArg1 arg1,
            TArg2 arg2,
            TArg3 arg3,
            CancellationToken cancellationToken);
    }
}