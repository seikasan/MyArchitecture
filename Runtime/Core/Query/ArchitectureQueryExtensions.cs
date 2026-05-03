using System.Runtime.CompilerServices;
using Cysharp.Threading.Tasks;

namespace MyArchitecture.Core
{
    public static class ArchitectureQueryExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TResult SendQuery<TQuery, TResult>(
            this ICanSendQuery source)
            where TQuery : IQuery<TResult>
        {
            return source.QueryRunner.Send<TQuery, TResult>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TResult SendQuery<TQuery, TArg, TResult>(
            this ICanSendQuery source,
            TArg arg)
            where TQuery : IQuery<TArg, TResult>
        {
            return source.QueryRunner.Send<TQuery, TArg, TResult>(arg);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UniTask<TResult> SendQueryAsync<TQuery, TResult>(
            this ICanSendQuery source)
            where TQuery : IAsyncQuery<TResult>
        {
            return source.QueryRunner.SendAsync<TQuery, TResult>(
                source.DisposeCancellationToken);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UniTask<TResult> SendQueryAsync<TQuery, TArg, TResult>(
            this ICanSendQuery source,
            TArg arg)
            where TQuery : IAsyncQuery<TArg, TResult>
        {
            return source.QueryRunner.SendAsync<TQuery, TArg, TResult>(
                arg,
                source.DisposeCancellationToken);
        }
    }
}
