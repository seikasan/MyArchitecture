using MyArchitecture.Core;

namespace MyArchitecture.Feature.ADV
{
    public readonly struct AdvSignalEvent : IEvent
    {
        public AdvSignalEvent(AdvSignal signal)
        {
            Signal = signal;
        }

        public AdvSignal Signal { get; }
    }
}