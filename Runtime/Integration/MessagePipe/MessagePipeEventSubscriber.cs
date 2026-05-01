using System;
using System.Collections.Generic;
using MessagePipe;
using MyArchitecture.Core;
using VContainer;

namespace MyArchitecture.Integration
{
    public sealed class MessagePipeEventSubscriber : IEventSubscriber
    {
        private readonly IObjectResolver _resolver;
        private readonly ArchitectureSettings _settings;
        private readonly IArchitectureLogger _logger;

        private readonly Dictionary<Type, object> _subscribers = new();

        public MessagePipeEventSubscriber(
            IObjectResolver resolver,
            ArchitectureSettings settings,
            IArchitectureLogger logger)
        {
            _resolver = resolver;
            _settings = settings;
            _logger = logger;
        }

        public IDisposable Subscribe<TEvent>(Action<TEvent> handler)
            where TEvent : IEvent
        {
            if (handler is null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            if (_settings.EnableEventSubscribeLog)
            {
                _logger.Log($"Event Subscribe: {typeof(TEvent).Name}");
            }

            return GetSubscriber<TEvent>().Subscribe(handler);
        }

        private ISubscriber<TEvent> GetSubscriber<TEvent>()
            where TEvent : IEvent
        {
            var eventType = typeof(TEvent);

            if (_subscribers.TryGetValue(eventType, out var subscriber))
            {
                return (ISubscriber<TEvent>)subscriber;
            }

            try
            {
                var resolved = _resolver.Resolve<ISubscriber<TEvent>>();
                _subscribers.Add(eventType, resolved);
                return resolved;
            }
            catch (Exception exception)
            {
                throw new ArchitectureEventBrokerNotRegisteredException(
                    typeof(TEvent),
                    typeof(ISubscriber<TEvent>),
                    exception);
            }
        }
    }
}