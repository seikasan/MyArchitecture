namespace MyArchitecture.Core
{
    public interface IEventPublisher
    {
        void Publish<TEvent>(TEvent eventData)
            where TEvent : IEvent;
    }
}