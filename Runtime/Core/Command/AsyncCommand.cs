using System.Threading;
using Cysharp.Threading.Tasks;

namespace MyArchitecture.Core
{
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
}
