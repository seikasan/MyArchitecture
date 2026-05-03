using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Cysharp.Threading.Tasks;
using VContainer;

namespace MyArchitecture.Core
{
    public sealed class QueryRunner : IQueryRunner
    {
        private readonly Dictionary<Type, object> _queries = new();
        private readonly IObjectResolver _resolver;
        private readonly QueryRegistry _registry;
        private readonly ArchitectureSettings _settings;
        private readonly IArchitectureLogger _logger;

        public QueryRunner(
            IObjectResolver resolver,
            QueryRegistry registry,
            ArchitectureSettings settings,
            IArchitectureLogger logger)
        {
            _resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
            _registry = registry ?? throw new ArgumentNullException(nameof(registry));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TResult Send<TQuery, TResult>()
            where TQuery : IQuery<TResult>
        {
            Log<TQuery, TResult>("Query");
            return Get<TQuery>().Execute();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TResult Send<TQuery, TArg, TResult>(TArg arg)
            where TQuery : IQuery<TArg, TResult>
        {
            Log<TQuery, TResult>("Query");
            return Get<TQuery>().Execute(arg);
        }

        public UniTask<TResult> SendAsync<TQuery, TResult>(
            CancellationToken cancellationToken = default)
            where TQuery : IAsyncQuery<TResult>
        {
            cancellationToken.ThrowIfCancellationRequested();
            Log<TQuery, TResult>("Async Query");
            return Get<TQuery>().ExecuteAsync(cancellationToken);
        }

        public UniTask<TResult> SendAsync<TQuery, TArg, TResult>(
            TArg arg,
            CancellationToken cancellationToken = default)
            where TQuery : IAsyncQuery<TArg, TResult>
        {
            cancellationToken.ThrowIfCancellationRequested();
            Log<TQuery, TResult>("Async Query");
            return Get<TQuery>().ExecuteAsync(arg, cancellationToken);
        }

        private TQuery Get<TQuery>()
        {
            var queryType = typeof(TQuery);

            if (_queries.TryGetValue(queryType, out var query))
            {
                return (TQuery)query;
            }

            if (!_registry.Contains(queryType))
            {
                throw new InvalidOperationException(
                    $"Query is not registered: {queryType.FullName}");
            }

            try
            {
                query = _resolver.Resolve(queryType);
                _queries.Add(queryType, query);
                return (TQuery)query;
            }
            catch (Exception exception)
            {
                throw new InvalidOperationException(
                    $"Query dependency resolution failed: {queryType.FullName}",
                    exception);
            }
        }

        private void Log<TQuery, TResult>(string prefix)
        {
            if (_settings.EnableQueryLog)
            {
                _logger.Log($"{prefix}: {typeof(TQuery).Name} -> {typeof(TResult).Name}");
            }
        }
    }
}
