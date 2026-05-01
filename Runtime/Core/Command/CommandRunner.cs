using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Cysharp.Threading.Tasks;
using VContainer;

namespace MyArchitecture.Core
{
    public sealed class CommandRunner : ICommandRunner
    {
        private readonly Dictionary<Type, object> _commands = new();
        private readonly IObjectResolver _resolver;
        private readonly CommandRegistry _registry;
        private readonly ArchitectureSettings _settings;
        private readonly IArchitectureLogger _logger;

        public CommandRunner(
            IObjectResolver resolver,
            CommandRegistry registry,
            ArchitectureSettings settings,
            IArchitectureLogger logger)
        {
            _resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
            _registry = registry ?? throw new ArgumentNullException(nameof(registry));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Send<TCommand>()
            where TCommand : ICommand
        {
            Log<TCommand>("Command");
            Get<TCommand>().Execute();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Send<TCommand, TArg>(TArg arg)
            where TCommand : ICommand<TArg>
        {
            Log<TCommand>("Command");
            Get<TCommand>().Execute(arg);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Send<TCommand, TArg1, TArg2>(TArg1 arg1, TArg2 arg2)
            where TCommand : ICommand<TArg1, TArg2>
        {
            Log<TCommand>("Command");
            Get<TCommand>().Execute(arg1, arg2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Send<TCommand, TArg1, TArg2, TArg3>(TArg1 arg1, TArg2 arg2, TArg3 arg3)
            where TCommand : ICommand<TArg1, TArg2,  TArg3>
        {
            Log<TCommand>("Command");
            Get<TCommand>().Execute(arg1, arg2, arg3);
        }

        public UniTask SendAsync<TCommand>(
            CancellationToken cancellationToken = default)
            where TCommand : IAsyncCommand
        {
            cancellationToken.ThrowIfCancellationRequested();
            Log<TCommand>("Async Command");
            return Get<TCommand>().ExecuteAsync(cancellationToken);
        }

        public UniTask SendAsync<TCommand, TArg>(
            TArg arg,
            CancellationToken cancellationToken = default)
            where TCommand : IAsyncCommand<TArg>
        {
            cancellationToken.ThrowIfCancellationRequested();
            Log<TCommand>("Async Command");
            return Get<TCommand>().ExecuteAsync(arg, cancellationToken);
        }

        public UniTask SendAsync<TCommand, TArg1, TArg2>(
            TArg1 arg1,
            TArg2 arg2,
            CancellationToken cancellationToken = default)
            where TCommand : IAsyncCommand<TArg1, TArg2>
        {
            cancellationToken.ThrowIfCancellationRequested();
            Log<TCommand>("Async Command");
            return Get<TCommand>().ExecuteAsync(arg1, arg2, cancellationToken);
        }

        public UniTask SendAsync<TCommand, TArg1, TArg2, TArg3>(
            TArg1 arg1,
            TArg2 arg2,
            TArg3 arg3,
            CancellationToken cancellationToken = default)
            where TCommand : IAsyncCommand<TArg1, TArg2, TArg3>
        {
            cancellationToken.ThrowIfCancellationRequested();
            Log<TCommand>("Async Command");
            return Get<TCommand>().ExecuteAsync(arg1, arg2, arg3, cancellationToken);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TrySend<TCommand>()
            where TCommand : ITryCommand
        {
            Log<TCommand>("TryCommand");
            return Get<TCommand>().TryExecute();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TrySend<TCommand, TArg>(TArg arg)
            where TCommand : ITryCommand<TArg>
        {
            Log<TCommand>("TryCommand");
            return Get<TCommand>().TryExecute(arg);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TrySend<TCommand, TArg1, TArg2>(TArg1 arg1, TArg2 arg2)
            where TCommand : ITryCommand<TArg1, TArg2>
        {
            Log<TCommand>("TryCommand");
            return Get<TCommand>().TryExecute(arg1, arg2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TrySend<TCommand, TArg1, TArg2, TArg3>(TArg1 arg1, TArg2 arg2, TArg3 arg3)
            where TCommand : ITryCommand<TArg1, TArg2, TArg3>
        {
            Log<TCommand>("TryCommand");
            return Get<TCommand>().TryExecute(arg1, arg2, arg3);
        }

        public UniTask<bool> TrySendAsync<TCommand>(
            CancellationToken cancellationToken = default)
            where TCommand : IAsyncTryCommand
        {
            cancellationToken.ThrowIfCancellationRequested();
            Log<TCommand>("Async TryCommand");
            return Get<TCommand>().TryExecuteAsync(cancellationToken);
        }

        public UniTask<bool> TrySendAsync<TCommand, TArg>(
            TArg arg,
            CancellationToken cancellationToken = default)
            where TCommand : IAsyncTryCommand<TArg>
        {
            cancellationToken.ThrowIfCancellationRequested();
            Log<TCommand>("Async TryCommand");
            return Get<TCommand>().TryExecuteAsync(arg, cancellationToken);
        }

        public UniTask<bool> TrySendAsync<TCommand, TArg1, TArg2>(
            TArg1 arg1,
            TArg2 arg2,
            CancellationToken cancellationToken = default)
            where TCommand : IAsyncTryCommand<TArg1, TArg2>
        {
            cancellationToken.ThrowIfCancellationRequested();
            Log<TCommand>("Async TryCommand");
            return Get<TCommand>().TryExecuteAsync(arg1, arg2, cancellationToken);
        }

        public UniTask<bool> TrySendAsync<TCommand, TArg1, TArg2, TArg3>(
            TArg1 arg1,
            TArg2 arg2,
            TArg3 arg3,
            CancellationToken cancellationToken = default)
            where TCommand : IAsyncTryCommand<TArg1, TArg2, TArg3>
        {
            cancellationToken.ThrowIfCancellationRequested();
            Log<TCommand>("Async TryCommand");
            return Get<TCommand>().TryExecuteAsync(arg1, arg2, arg3, cancellationToken);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TResult SendResult<TCommand, TResult>()
            where TCommand : IResultCommand<TResult>
            where TResult : ICommandResult
        {
            Log<TCommand>("ResultCommand");
            return Get<TCommand>().Execute();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TResult SendResult<TCommand, TArg, TResult>(TArg arg)
            where TCommand : IResultCommand<TArg, TResult>
            where TResult : ICommandResult
        {
            Log<TCommand>("ResultCommand");
            return Get<TCommand>().Execute(arg);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TResult SendResult<TCommand, TArg1, TArg2, TResult>(TArg1 arg1, TArg2 arg2)
            where TCommand : IResultCommand<TArg1, TArg2, TResult>
            where TResult : ICommandResult
        {
            Log<TCommand>("ResultCommand");
            return Get<TCommand>().Execute(arg1, arg2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TResult SendResult<TCommand, TArg1, TArg2, TArg3, TResult>(TArg1 arg1, TArg2 arg2, TArg3 arg3)
            where TCommand : IResultCommand<TArg1, TArg2, TArg3, TResult>
            where TResult : ICommandResult
        {
            Log<TCommand>("ResultCommand");
            return Get<TCommand>().Execute(arg1, arg2, arg3);
        }

        public UniTask<TResult> SendResultAsync<TCommand, TResult>(
            CancellationToken cancellationToken = default)
            where TCommand : IAsyncResultCommand<TResult>
            where TResult : ICommandResult
        {
            cancellationToken.ThrowIfCancellationRequested();
            Log<TCommand>("Async ResultCommand");
            return Get<TCommand>().ExecuteAsync(cancellationToken);
        }

        public UniTask<TResult> SendResultAsync<TCommand, TArg, TResult>(
            TArg arg,
            CancellationToken cancellationToken = default)
            where TCommand : IAsyncResultCommand<TArg, TResult>
            where TResult : ICommandResult
        {
            cancellationToken.ThrowIfCancellationRequested();
            Log<TCommand>("Async ResultCommand");
            return Get<TCommand>().ExecuteAsync(arg, cancellationToken);
        }

        public UniTask<TResult> SendResultAsync<TCommand, TArg1, TArg2, TResult>(
            TArg1 arg1,
            TArg2 arg2,
            CancellationToken cancellationToken = default)
            where TCommand : IAsyncResultCommand<TArg1, TArg2, TResult>
            where TResult : ICommandResult
        {
            cancellationToken.ThrowIfCancellationRequested();
            Log<TCommand>("Async ResultCommand");
            return Get<TCommand>().ExecuteAsync(arg1, arg2, cancellationToken);
        }

        public UniTask<TResult> SendResultAsync<TCommand, TArg1, TArg2, TArg3, TResult>(
            TArg1 arg1,
            TArg2 arg2,
            TArg3 arg3,
            CancellationToken cancellationToken = default)
            where TCommand : IAsyncResultCommand<TArg1, TArg2, TArg3, TResult>
            where TResult : ICommandResult
        {
            cancellationToken.ThrowIfCancellationRequested();
            Log<TCommand>("Async ResultCommand");
            return Get<TCommand>().ExecuteAsync(arg1, arg2, arg3, cancellationToken);
        }

        private TCommand Get<TCommand>()
        {
            var commandType = typeof(TCommand);

            if (_commands.TryGetValue(commandType, out var command))
            {
                return (TCommand)command;
            }

            if (!_registry.Contains(commandType))
            {
                throw new InvalidOperationException(
                    $"Command is not registered: {commandType.FullName}");
            }

            try
            {
                command = _resolver.Resolve(commandType);
                _commands.Add(commandType, command);
                return (TCommand)command;
            }
            catch (Exception exception)
            {
                throw new InvalidOperationException(
                    $"Command dependency resolution failed: {commandType.FullName}",
                    exception);
            }
        }

        private void Log<TCommand>(string prefix)
        {
            if (_settings.EnableCommandLog)
            {
                _logger.Log($"{prefix}: {typeof(TCommand).Name}");
            }
        }
    }
}
