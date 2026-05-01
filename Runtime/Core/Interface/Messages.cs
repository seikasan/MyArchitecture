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

    public interface ICommand<in TArg1, in TArg2> : ICanSendCommand
    {
        void Execute(TArg1 arg1, TArg2 arg2);
    }

    public interface ICommand<in TArg1, in TArg2, in TArg3> : ICanSendCommand
    {
        void Execute(TArg1 arg1, TArg2 arg2, TArg3 arg3);
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

    public interface IAsyncCommand<in TArg1, in TArg2> : ICanSendCommand
    {
        UniTask ExecuteAsync(
            TArg1 arg1,
            TArg2 arg2,
            CancellationToken cancellationToken);
    }

    public interface IAsyncCommand<in TArg1, in TArg2, in TArg3> : ICanSendCommand
    {
        UniTask ExecuteAsync(
            TArg1 arg1,
            TArg2 arg2,
            TArg3 arg3,
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

    public interface ITryCommand<in TArg1, in TArg2> : ICanSendCommand
    {
        bool TryExecute(TArg1 arg1, TArg2 arg2);
    }

    public interface ITryCommand<in TArg1, in TArg2, in TArg3> : ICanSendCommand
    {
        bool TryExecute(TArg1 arg1, TArg2 arg2, TArg3 arg3);
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

    public interface IAsyncTryCommand<in TArg1, in TArg2> : ICanSendCommand
    {
        UniTask<bool> TryExecuteAsync(
            TArg1 arg1,
            TArg2 arg2,
            CancellationToken cancellationToken);
    }

    public interface IAsyncTryCommand<in TArg1, in TArg2, in TArg3> : ICanSendCommand
    {
        UniTask<bool> TryExecuteAsync(
            TArg1 arg1,
            TArg2 arg2,
            TArg3 arg3,
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

    public interface IResultCommand<in TArg1, in TArg2, TResult> : ICanSendCommand
        where TResult : ICommandResult
    {
        TResult Execute(TArg1 arg1, TArg2 arg2);
    }

    public interface IResultCommand<in TArg1, in TArg2, in TArg3, TResult> : ICanSendCommand
        where TResult : ICommandResult
    {
        TResult Execute(TArg1 arg1, TArg2 arg2, TArg3 arg3);
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

    public interface IAsyncResultCommand<in TArg1, in TArg2, TResult> : ICanSendCommand
        where TResult : ICommandResult
    {
        UniTask<TResult> ExecuteAsync(
            TArg1 arg1,
            TArg2 arg2,
            CancellationToken cancellationToken);
    }

    public interface IAsyncResultCommand<in TArg1, in TArg2, in TArg3, TResult> : ICanSendCommand
        where TResult : ICommandResult
    {
        UniTask<TResult> ExecuteAsync(
            TArg1 arg1,
            TArg2 arg2,
            TArg3 arg3,
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

        void Send<TCommand, TArg1, TArg2>(
            TArg1 arg1,
            TArg2 arg2)
            where TCommand : ICommand<TArg1, TArg2>;

        void Send<TCommand, TArg1, TArg2, TArg3>(
            TArg1 arg1,
            TArg2 arg2,
            TArg3 arg3)
            where TCommand : ICommand<TArg1, TArg2, TArg3>;

        UniTask SendAsync<TCommand>(
            CancellationToken cancellationToken = default)
            where TCommand : IAsyncCommand;

        UniTask SendAsync<TCommand, TArg>(
            TArg arg,
            CancellationToken cancellationToken = default)
            where TCommand : IAsyncCommand<TArg>;

        UniTask SendAsync<TCommand, TArg1, TArg2>(
            TArg1 arg1,
            TArg2 arg2,
            CancellationToken cancellationToken = default)
            where TCommand : IAsyncCommand<TArg1, TArg2>;

        UniTask SendAsync<TCommand, TArg1, TArg2, TArg3>(
            TArg1 arg1,
            TArg2 arg2,
            TArg3 arg3,
            CancellationToken cancellationToken = default)
            where TCommand : IAsyncCommand<TArg1, TArg2, TArg3>;

        bool TrySend<TCommand>()
            where TCommand : ITryCommand;

        bool TrySend<TCommand, TArg>(TArg arg)
            where TCommand : ITryCommand<TArg>;

        bool TrySend<TCommand, TArg1, TArg2>(TArg1 arg1, TArg2 arg2)
            where TCommand : ITryCommand<TArg1, TArg2>;

        bool TrySend<TCommand, TArg1, TArg2, TArg3>(TArg1 arg1, TArg2 arg2, TArg3 arg3)
            where TCommand : ITryCommand<TArg1, TArg2, TArg3>;

        UniTask<bool> TrySendAsync<TCommand>(
            CancellationToken cancellationToken = default)
            where TCommand : IAsyncTryCommand;

        UniTask<bool> TrySendAsync<TCommand, TArg>(
            TArg arg,
            CancellationToken cancellationToken = default)
            where TCommand : IAsyncTryCommand<TArg>;

        UniTask<bool> TrySendAsync<TCommand, TArg1, TArg2>(
            TArg1 arg1,
            TArg2 arg2,
            CancellationToken cancellationToken = default)
            where TCommand : IAsyncTryCommand<TArg1, TArg2>;

        UniTask<bool> TrySendAsync<TCommand, TArg1, TArg2, TArg3>(
            TArg1 arg1,
            TArg2 arg2,
            TArg3 arg3,
            CancellationToken cancellationToken = default)
            where TCommand : IAsyncTryCommand<TArg1, TArg2, TArg3>;

        TResult SendResult<TCommand, TResult>()
            where TCommand : IResultCommand<TResult>
            where TResult : ICommandResult;

        TResult SendResult<TCommand, TArg, TResult>(TArg arg)
            where TCommand : IResultCommand<TArg, TResult>
            where TResult : ICommandResult;

        TResult SendResult<TCommand, TArg1, TArg2, TResult>(TArg1 arg1, TArg2 arg2)
            where TCommand : IResultCommand<TArg1, TArg2, TResult>
            where TResult : ICommandResult;

        TResult SendResult<TCommand, TArg1, TArg2, TArg3, TResult>(TArg1 arg1, TArg2 arg2, TArg3 arg3)
            where TCommand : IResultCommand<TArg1, TArg2, TArg3, TResult>
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

        UniTask<TResult> SendResultAsync<TCommand, TArg1, TArg2, TResult>(
            TArg1 arg1,
            TArg2 arg2,
            CancellationToken cancellationToken = default)
            where TCommand : IAsyncResultCommand<TArg1, TArg2, TResult>
            where TResult : ICommandResult;

        UniTask<TResult> SendResultAsync<TCommand, TArg1, TArg2, TArg3, TResult>(
            TArg1 arg1,
            TArg2 arg2,
            TArg3 arg3,
            CancellationToken cancellationToken = default)
            where TCommand : IAsyncResultCommand<TArg1, TArg2, TArg3, TResult>
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

    public interface IQuery<in TArg1, in TArg2, TResult>
    {
        TResult Execute(TArg1 arg1, TArg2 arg2);
    }

    public interface IQuery<in TArg1, in TArg2, in TArg3, TResult>
    {
        TResult Execute(TArg1 arg1, TArg2 arg2, TArg3 arg3);
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

    public interface IAsyncQuery<in TArg1, in TArg2, TResult>
    {
        UniTask<TResult> ExecuteAsync(
            TArg1 arg1,
            TArg2 arg2,
            CancellationToken cancellationToken);
    }

    public interface IAsyncQuery<in TArg1, in TArg2, in TArg3, TResult>
    {
        UniTask<TResult> ExecuteAsync(
            TArg1 arg1,
            TArg2 arg2,
            TArg3 arg3,
            CancellationToken cancellationToken);
    }

    // --- IQueryRunner ---

    public interface IQueryRunner
    {
        TResult Send<TQuery, TResult>()
            where TQuery : IQuery<TResult>;

        TResult Send<TQuery, TArg, TResult>(TArg arg)
            where TQuery : IQuery<TArg, TResult>;

        TResult Send<TQuery, TArg1, TArg2, TResult>(TArg1 arg1, TArg2 arg2)
            where TQuery : IQuery<TArg1, TArg2, TResult>;

        TResult Send<TQuery, TArg1, TArg2, TArg3, TResult>(TArg1 arg1, TArg2 arg2, TArg3 arg3)
            where TQuery : IQuery<TArg1, TArg2, TArg3, TResult>;

        UniTask<TResult> SendAsync<TQuery, TResult>(
            CancellationToken cancellationToken = default)
            where TQuery : IAsyncQuery<TResult>;

        UniTask<TResult> SendAsync<TQuery, TArg, TResult>(
            TArg arg,
            CancellationToken cancellationToken = default)
            where TQuery : IAsyncQuery<TArg, TResult>;

        UniTask<TResult> SendAsync<TQuery, TArg1, TArg2, TResult>(
            TArg1 arg1,
            TArg2 arg2,
            CancellationToken cancellationToken = default)
            where TQuery : IAsyncQuery<TArg1, TArg2, TResult>;

        UniTask<TResult> SendAsync<TQuery, TArg1, TArg2, TArg3, TResult>(
            TArg1 arg1,
            TArg2 arg2,
            TArg3 arg3,
            CancellationToken cancellationToken = default)
            where TQuery : IAsyncQuery<TArg1, TArg2, TArg3, TResult>;
    }
}
