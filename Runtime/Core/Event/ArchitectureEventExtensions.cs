using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace MyArchitecture.Core
{
    public static class ArchitectureEventExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PublishEvent<TEvent>(
            this ICanPublishEvent source,
            TEvent eventData)
            where TEvent : IEvent
        {
            source.EventPublisher.Publish(eventData);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SubscribeEvent<TEvent>(
            this ICanSubscribeEvent source,
            Action<TEvent> handler)
            where TEvent : IEvent
        {
            source.Track(source.EventSubscriber.Subscribe(handler));
        }

        public static void SubscribeEvent<TEvent>(
            this ICanSubscribeEvent source,
            Func<TEvent, CancellationToken, UniTask> asyncHandler)
            where TEvent : IEvent
        {
            if (asyncHandler == null)
            {
                throw new ArgumentNullException(nameof(asyncHandler));
            }

            var ct = source.DisposeCancellationToken;

            source.Track(source.EventSubscriber.Subscribe<TEvent>(e =>
            {
                asyncHandler(e, ct).Forget();
            }));
        }
    }
}
