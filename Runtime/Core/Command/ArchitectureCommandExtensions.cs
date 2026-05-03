using System.Runtime.CompilerServices;
using Cysharp.Threading.Tasks;

namespace MyArchitecture.Core
{
    public static class ArchitectureCommandExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SendCommand<TCommand>(this ICanSendCommand source)
            where TCommand : ICommand
        {
            source.CommandRunner.Send<TCommand>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SendCommand<TCommand, TArg>(
            this ICanSendCommand source,
            TArg arg)
            where TCommand : ICommand<TArg>
        {
            source.CommandRunner.Send<TCommand, TArg>(arg);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UniTask SendCommandAsync<TCommand>(
            this ICanSendCommand source)
            where TCommand : IAsyncCommand
        {
            return source.CommandRunner.SendAsync<TCommand>(
                source.DisposeCancellationToken);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UniTask SendCommandAsync<TCommand, TArg>(
            this ICanSendCommand source,
            TArg arg)
            where TCommand : IAsyncCommand<TArg>
        {
            return source.CommandRunner.SendAsync<TCommand, TArg>(
                arg,
                source.DisposeCancellationToken);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TrySendCommand<TCommand>(
            this ICanSendCommand source)
            where TCommand : ITryCommand
        {
            return source.CommandRunner.TrySend<TCommand>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TrySendCommand<TCommand, TArg>(
            this ICanSendCommand source,
            TArg arg)
            where TCommand : ITryCommand<TArg>
        {
            return source.CommandRunner.TrySend<TCommand, TArg>(arg);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UniTask<bool> TrySendCommandAsync<TCommand>(
            this ICanSendCommand source)
            where TCommand : IAsyncTryCommand
        {
            return source.CommandRunner.TrySendAsync<TCommand>(
                source.DisposeCancellationToken);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UniTask<bool> TrySendCommandAsync<TCommand, TArg>(
            this ICanSendCommand source,
            TArg arg)
            where TCommand : IAsyncTryCommand<TArg>
        {
            return source.CommandRunner.TrySendAsync<TCommand, TArg>(
                arg,
                source.DisposeCancellationToken);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TResult SendResultCommand<TCommand, TResult>(
            this ICanSendCommand source)
            where TCommand : IResultCommand<TResult>
            where TResult : ICommandResult
        {
            return source.CommandRunner.SendResult<TCommand, TResult>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TResult SendResultCommand<TCommand, TArg, TResult>(
            this ICanSendCommand source,
            TArg arg)
            where TCommand : IResultCommand<TArg, TResult>
            where TResult : ICommandResult
        {
            return source.CommandRunner.SendResult<TCommand, TArg, TResult>(arg);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UniTask<TResult> SendResultCommandAsync<TCommand, TResult>(
            this ICanSendCommand source)
            where TCommand : IAsyncResultCommand<TResult>
            where TResult : ICommandResult
        {
            return source.CommandRunner.SendResultAsync<TCommand, TResult>(
                source.DisposeCancellationToken);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UniTask<TResult> SendResultCommandAsync<TCommand, TArg, TResult>(
            this ICanSendCommand source,
            TArg arg)
            where TCommand : IAsyncResultCommand<TArg, TResult>
            where TResult : ICommandResult
        {
            return source.CommandRunner.SendResultAsync<TCommand, TArg, TResult>(
                arg,
                source.DisposeCancellationToken);
        }
    }
}
