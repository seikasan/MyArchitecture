using System.Threading;
using Cysharp.Threading.Tasks;

namespace MyArchitecture.Core
{
    // --- ICommands ---

    public interface ICommand : ICanSendCommand
    {
        void Execute();
    }

    public interface ICommand<in TArg> : ICanSendCommand
    {
        void Execute(TArg arg);
    }

    // --- IAsyncCommands ---

    public interface IAsyncCommand : ICanSendCommand
    {
        UniTask ExecuteAsync(CancellationToken cancellationToken);
    }

    public interface IAsyncCommand<in TArg> : ICanSendCommand
    {
        UniTask ExecuteAsync(
            TArg arg,
            CancellationToken cancellationToken);
    }

    // --- ITryCommands ---

    public interface ITryCommand : ICanSendCommand
    {
        bool TryExecute();
    }

    public interface ITryCommand<in TArg> : ICanSendCommand
    {
        bool TryExecute(TArg arg);
    }

    // --- IAsyncTryCommands ---

    public interface IAsyncTryCommand : ICanSendCommand
    {
        UniTask<bool> TryExecuteAsync(
            CancellationToken cancellationToken);
    }

    public interface IAsyncTryCommand<in TArg> : ICanSendCommand
    {
        UniTask<bool> TryExecuteAsync(
            TArg arg,
            CancellationToken cancellationToken);
    }

    // --- IResultCommand ---

    public interface IResultCommand<TResult> : ICanSendCommand
        where TResult : ICommandResult
    {
        TResult Execute();
    }

    public interface IResultCommand<in TArg, TResult> : ICanSendCommand
        where TResult : ICommandResult
    {
        TResult Execute(TArg arg);
    }

    // --- IAsyncResultCommand ---

    public interface IAsyncResultCommand<TResult> : ICanSendCommand
        where TResult : ICommandResult
    {
        UniTask<TResult> ExecuteAsync(CancellationToken cancellationToken);
    }

    public interface IAsyncResultCommand<in TArg, TResult> : ICanSendCommand
        where TResult : ICommandResult
    {
        UniTask<TResult> ExecuteAsync(
            TArg arg,
            CancellationToken cancellationToken);
    }

    // --- ICommandResult ---

    public interface ICommandResult
    {
        bool Success { get; }
    }

    // --- ICommandRunner ---

    public interface ICommandRunner
    {
        void Send<TCommand>()
            where TCommand : ICommand;

        void Send<TCommand, TArg>(TArg arg)
            where TCommand : ICommand<TArg>;

        UniTask SendAsync<TCommand>(
            CancellationToken cancellationToken = default)
            where TCommand : IAsyncCommand;

        UniTask SendAsync<TCommand, TArg>(
            TArg arg,
            CancellationToken cancellationToken = default)
            where TCommand : IAsyncCommand<TArg>;

        bool TrySend<TCommand>()
            where TCommand : ITryCommand;

        bool TrySend<TCommand, TArg>(TArg arg)
            where TCommand : ITryCommand<TArg>;

        UniTask<bool> TrySendAsync<TCommand>(
            CancellationToken cancellationToken = default)
            where TCommand : IAsyncTryCommand;

        UniTask<bool> TrySendAsync<TCommand, TArg>(
            TArg arg,
            CancellationToken cancellationToken = default)
            where TCommand : IAsyncTryCommand<TArg>;

        TResult SendResult<TCommand, TResult>()
            where TCommand : IResultCommand<TResult>
            where TResult : ICommandResult;

        TResult SendResult<TCommand, TArg, TResult>(TArg arg)
            where TCommand : IResultCommand<TArg, TResult>
            where TResult : ICommandResult;

        UniTask<TResult> SendResultAsync<TCommand, TResult>(
            CancellationToken cancellationToken = default)
            where TCommand : IAsyncResultCommand<TResult>
            where TResult : ICommandResult;

        UniTask<TResult> SendResultAsync<TCommand, TArg, TResult>(
            TArg arg,
            CancellationToken cancellationToken = default)
            where TCommand : IAsyncResultCommand<TArg, TResult>
            where TResult : ICommandResult;
    }

    // --- IQueries ---

    public interface IQuery<TResult>
    {
        TResult Execute();
    }

    public interface IQuery<in TArg, TResult>
    {
        TResult Execute(TArg arg);
    }

    // --- IAsyncQueries ---

    public interface IAsyncQuery<TResult>
    {
        UniTask<TResult> ExecuteAsync(
            CancellationToken cancellationToken);
    }

    public interface IAsyncQuery<in TArg, TResult>
    {
        UniTask<TResult> ExecuteAsync(
            TArg arg,
            CancellationToken cancellationToken);
    }

    // --- IQueryRunner ---

    public interface IQueryRunner
    {
        TResult Send<TQuery, TResult>()
            where TQuery : IQuery<TResult>;

        TResult Send<TQuery, TArg, TResult>(TArg arg)
            where TQuery : IQuery<TArg, TResult>;

        UniTask<TResult> SendAsync<TQuery, TResult>(
            CancellationToken cancellationToken = default)
            where TQuery : IAsyncQuery<TResult>;

        UniTask<TResult> SendAsync<TQuery, TArg, TResult>(
            TArg arg,
            CancellationToken cancellationToken = default)
            where TQuery : IAsyncQuery<TArg, TResult>;
    }
}
