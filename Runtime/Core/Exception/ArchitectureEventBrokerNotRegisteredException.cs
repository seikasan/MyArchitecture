using System;

namespace MyArchitecture.Core
{
    public sealed class ArchitectureEventBrokerNotRegisteredException : Exception
    {
        public Type EventType { get; }
        public Type BrokerType { get; }

        public ArchitectureEventBrokerNotRegisteredException(
            Type eventType,
            Type brokerType,
            Exception innerException)
            : base(
                $"Event broker is not registered. Event: {eventType.FullName}, Broker: {brokerType.FullName}",
                innerException)
        {
            EventType = eventType;
            BrokerType = brokerType;
        }
    }
}