using System;
using MyArchitecture.Core;
using R3;

namespace MyArchitecture.Integration
{
    public static class ViewSignalR3Extensions
    {
        public static void SubscribeViewSignal(
            this IHasArchitectureLifetime source,
            ViewSignal signal,
            Action handler)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (signal == null) throw new ArgumentNullException(nameof(signal));
            if (handler == null) throw new ArgumentNullException(nameof(handler));

            source.Track(signal.AsObservable().Subscribe(_ => handler()));
        }

        public static void SubscribeViewSignal<T>(
            this IHasArchitectureLifetime source,
            ViewSignal<T> signal,
            Action<T> handler)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (signal == null) throw new ArgumentNullException(nameof(signal));
            if (handler == null) throw new ArgumentNullException(nameof(handler));

            source.Track(signal.AsObservable().Subscribe(handler));
        }

        public static void SubscribeViewSignalThrottled(
            this IHasArchitectureLifetime source,
            ViewSignal signal,
            Action handler,
            TimeSpan interval)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (signal == null) throw new ArgumentNullException(nameof(signal));
            if (handler == null) throw new ArgumentNullException(nameof(handler));

            source.Track(
                signal.AsObservable()
                    .ThrottleFirst(interval)
                    .Subscribe(_ => handler()));
        }

        public static void SubscribeViewSignalThrottled<T>(
            this IHasArchitectureLifetime source,
            ViewSignal<T> signal,
            Action<T> handler,
            TimeSpan interval)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (signal == null) throw new ArgumentNullException(nameof(signal));
            if (handler == null) throw new ArgumentNullException(nameof(handler));

            source.Track(
                signal.AsObservable()
                    .ThrottleFirst(interval)
                    .Subscribe(handler));
        }
    }
}
