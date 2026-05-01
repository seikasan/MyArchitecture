using System;
using System.Collections.Generic;
using MessagePipe;
using MyArchitecture.Core;
using VContainer;

namespace MyArchitecture.Integration
{
    public sealed class MessagePipeEventPublisher : IEventPublisher
    {
        private readonly IObjectResolver _resolver;
        private readonly ArchitectureSettings _settings;
        private readonly IArchitectureLogger _logger;

        private readonly Dictionary<Type, object> _publishers = new();

        public MessagePipeEventPublisher(
            IObjectResolver resolver,
            ArchitectureSettings settings,
            IArchitectureLogger logger)
        {
            _resolver = resolver;
            _settings = settings;
            _logger = logger;
        }

        public void Publish<TEvent>(TEvent eventData)
            where TEvent : IEvent
        {
            if (eventData is null)
            {
                throw new ArgumentNullException(nameof(eventData));
            }

            if (_settings.EnableEventPublishLog)
            {
                _logger.Log($"Event Publish: {typeof(TEvent).Name}");
            }

            GetPublisher<TEvent>().Publish(eventData);
        }

        private IPublisher<TEvent> GetPublisher<TEvent>()
            where TEvent : IEvent
        {
            var eventType = typeof(TEvent);

            if (_publishers.TryGetValue(eventType, out var publisher))
            {
                return (IPublisher<TEvent>)publisher;
            }

            try
            {
                var resolved = _resolver.Resolve<IPublisher<TEvent>>();
                _publishers.Add(eventType, resolved);
                return resolved;
            }
            catch (Exception exception)
            {
                throw new ArchitectureEventBrokerNotRegisteredException(
                    typeof(TEvent),
                    typeof(IPublisher<TEvent>),
                    exception);
            }
        }
    }
}