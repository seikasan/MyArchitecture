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
        public static TResult SendQuery<TQuery, TArg1, TArg2, TResult>(
            this ICanSendQuery source,
            TArg1 arg1,
            TArg2 arg2)
            where TQuery : IQuery<TArg1, TArg2, TResult>
        {
            return source.QueryRunner.Send<TQuery, TArg1, TArg2, TResult>(arg1, arg2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TResult SendQuery<TQuery, TArg1, TArg2, TArg3, TResult>(
            this ICanSendQuery source,
            TArg1 arg1,
            TArg2 arg2,
            TArg3 arg3)
            where TQuery : IQuery<TArg1, TArg2, TArg3, TResult>
        {
            return source.QueryRunner.Send<TQuery, TArg1, TArg2, TArg3, TResult>(arg1, arg2, arg3);
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UniTask<TResult> SendQueryAsync<TQuery, TArg1, TArg2, TResult>(
            this ICanSendQuery source,
            TArg1 arg1,
            TArg2 arg2)
            where TQuery : IAsyncQuery<TArg1, TArg2, TResult>
        {
            return source.QueryRunner.SendAsync<TQuery, TArg1, TArg2, TResult>(
                arg1,
                arg2,
                source.DisposeCancellationToken);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UniTask<TResult> SendQueryAsync<TQuery, TArg1, TArg2, TArg3, TResult>(
            this ICanSendQuery source,
            TArg1 arg1,
            TArg2 arg2,
            TArg3 arg3)
            where TQuery : IAsyncQuery<TArg1, TArg2, TArg3, TResult>
        {
            return source.QueryRunner.SendAsync<TQuery, TArg1, TArg2, TArg3, TResult>(
                arg1,
                arg2,
                arg3,
                source.DisposeCancellationToken);
        }
    }
}
