using System.Threading;
using Cysharp.Threading.Tasks;

namespace MyArchitecture.Core
{
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
}
