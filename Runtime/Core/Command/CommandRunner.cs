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
