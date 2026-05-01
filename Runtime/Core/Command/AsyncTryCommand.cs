using System.Threading;
using Cysharp.Threading.Tasks;

namespace MyArchitecture.Core
{
    // 0
    public abstract class AsyncTryCommand :
        CommandBase,
        IAsyncTryCommand
    {
        public UniTask<bool> TryExecuteAsync(
            CancellationToken cancellationToken)
        {
            return OnTryExecuteAsync(cancellationToken);
        }

        protected abstract UniTask<bool> OnTryExecuteAsync(
            CancellationToken cancellationToken);
    }

    // 1
    public abstract class AsyncTryCommand<TArg> :
        CommandBase,
        IAsyncTryCommand<TArg>
    {
        public UniTask<bool> TryExecuteAsync(
            TArg arg,
            CancellationToken cancellationToken)
        {
            return OnTryExecuteAsync(arg, cancellationToken);
        }

        protected abstract UniTask<bool> OnTryExecuteAsync(
            TArg arg,
            CancellationToken cancellationToken);
    }

    // 2
    public abstract class AsyncTryCommand<TArg1,  TArg2> :
        CommandBase,
        IAsyncTryCommand<TArg1, TArg2>
    {
        public UniTask<bool> TryExecuteAsync(
            TArg1 arg1,
            TArg2 arg2,
            CancellationToken cancellationToken)
        {
            return OnTryExecuteAsync(arg1, arg2, cancellationToken);
        }

        protected abstract UniTask<bool> OnTryExecuteAsync(
            TArg1 arg1,
            TArg2 arg2,
            CancellationToken cancellationToken);
    }

    // 3
    public abstract class AsyncTryCommand<TArg1,  TArg2, TArg3> :
        CommandBase,
        IAsyncTryCommand<TArg1, TArg2, TArg3>
    {
        public UniTask<bool> TryExecuteAsync(
            TArg1 arg1,
            TArg2 arg2,
            TArg3 arg3,
            CancellationToken cancellationToken)
        {
            return OnTryExecuteAsync(arg1, arg2, arg3, cancellationToken);
        }

        protected abstract UniTask<bool> OnTryExecuteAsync(
            TArg1 arg1,
            TArg2 arg2,
            TArg3 arg3,
            CancellationToken cancellationToken);
    }
}
