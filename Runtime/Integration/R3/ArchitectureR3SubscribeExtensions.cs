using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MyArchitecture.Core;
using R3;

namespace MyArchitecture.Integration
{
    public static class ArchitectureR3SubscribeExtensions
    {
        public static void SubscribeTo<T>(
            this IHasArchitectureLifetime source,
            Observable<T> observable,
            Action<T> handler)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (observable == null) throw new ArgumentNullException(nameof(observable));
            if (handler == null) throw new ArgumentNullException(nameof(handler));

            source.Track(observable.Subscribe(handler));
        }

        public static void SubscribeTo(
            this IHasArchitectureLifetime source,
            Observable<Unit> observable,
            Action handler)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (observable == null) throw new ArgumentNullException(nameof(observable));
            if (handler == null) throw new ArgumentNullException(nameof(handler));

            source.Track(observable.Subscribe(_ => handler()));
        }

        public static void SubscribeToAsync<T>(
            this IHasArchitectureLifetime source,
            Observable<T> observable,
            Func<T, CancellationToken, UniTask> asyncHandler)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (observable == null) throw new ArgumentNullException(nameof(observable));
            if (asyncHandler == null) throw new ArgumentNullException(nameof(asyncHandler));

            var cancellationToken = source.DisposeCancellationToken;

            source.Track(observable.Subscribe(value =>
            {
                asyncHandler(value, cancellationToken).Forget();
            }));
        }

        public static void SubscribeToThrottled<T>(
            this IHasArchitectureLifetime source,
            Observable<T> observable,
            Action<T> handler,
            TimeSpan interval)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (observable == null) throw new ArgumentNullException(nameof(observable));
            if (handler == null) throw new ArgumentNullException(nameof(handler));

            source.Track(
                observable
                    .ThrottleFirst(interval)
                    .Subscribe(handler));
        }

        public static void SubscribeToThrottled(
            this IHasArchitectureLifetime source,
            Observable<Unit> observable,
            Action handler,
            TimeSpan interval)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (observable == null) throw new ArgumentNullException(nameof(observable));
            if (handler == null) throw new ArgumentNullException(nameof(handler));

            source.Track(
                observable
                    .ThrottleFirst(interval)
                    .Subscribe(_ => handler()));
        }

        public static void SubscribeTo(
            this IHasArchitectureLifetime source,
            ViewSignal signal,
            Action handler)
        {
            if (signal == null) throw new ArgumentNullException(nameof(signal));

            source.SubscribeTo(signal.AsObservable(), handler);
        }

        public static void SubscribeTo<T>(
            this IHasArchitectureLifetime source,
            ViewSignal<T> signal,
            Action<T> handler)
        {
            if (signal == null) throw new ArgumentNullException(nameof(signal));

            source.SubscribeTo(signal.AsObservable(), handler);
        }

        public static void SubscribeToThrottled(
            this IHasArchitectureLifetime source,
            ViewSignal signal,
            Action handler,
            TimeSpan interval)
        {
            if (signal == null) throw new ArgumentNullException(nameof(signal));

            source.SubscribeToThrottled(signal.AsObservable(), handler, interval);
        }

        public static void SubscribeToThrottled<T>(
            this IHasArchitectureLifetime source,
            ViewSignal<T> signal,
            Action<T> handler,
            TimeSpan interval)
        {
            if (signal == null) throw new ArgumentNullException(nameof(signal));

            source.SubscribeToThrottled(signal.AsObservable(), handler, interval);
        }
    }
}
