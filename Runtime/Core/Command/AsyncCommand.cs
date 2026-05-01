using System.Threading;
using Cysharp.Threading.Tasks;

namespace MyArchitecture.Core
{
    // 0
    public abstract class AsyncCommand :
        CommandBase,
        IAsyncCommand
    {
        public UniTask ExecuteAsync(CancellationToken cancellationToken)
        {
            return OnExecuteAsync(cancellationToken);
        }

        protected abstract UniTask OnExecuteAsync(
            CancellationToken cancellationToken);
    }

    // 1
    public abstract class AsyncCommand<TArg> :
        CommandBase,
        IAsyncCommand<TArg>
    {
        public UniTask ExecuteAsync(
            TArg arg,
            CancellationToken cancellationToken)
        {
            return OnExecuteAsync(arg, cancellationToken);
        }

        protected abstract UniTask OnExecuteAsync(
            TArg arg,
            CancellationToken cancellationToken);
    }

    // 2
    public abstract class AsyncCommand<TArg1,  TArg2> :
        CommandBase,
        IAsyncCommand<TArg1, TArg2>
    {
        public UniTask ExecuteAsync(
            TArg1 arg1,
            TArg2 arg2,
            CancellationToken cancellationToken)
        {
            return OnExecuteAsync(arg1, arg2, cancellationToken);
        }

        protected abstract UniTask OnExecuteAsync(
            TArg1 arg1,
            TArg2 arg2,
            CancellationToken cancellationToken);
    }

    // 3
    public abstract class AsyncCommand<TArg1,  TArg2, TArg3> :
        CommandBase,
        IAsyncCommand<TArg1, TArg2, TArg3>
    {
        public UniTask ExecuteAsync(
            TArg1 arg1,
            TArg2 arg2,
            TArg3 arg3,
            CancellationToken cancellationToken)
        {
            return OnExecuteAsync(arg1, arg2, arg3, cancellationToken);
        }

        protected abstract UniTask OnExecuteAsync(
            TArg1 arg1,
            TArg2 arg2,
            TArg3 arg3,
            CancellationToken cancellationToken);
    }
}
