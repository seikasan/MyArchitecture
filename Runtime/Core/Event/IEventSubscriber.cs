using System;

namespace MyArchitecture.Core
{
    public interface IEventSubscriber
    {
        IDisposable Subscribe<TEvent>(Action<TEvent> handler)
            where TEvent : IEvent;
    }
}