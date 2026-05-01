using MyArchitecture.Core;

namespace MyArchitecture.Feature.ADV
{
    public readonly struct AdvLineChangedEvent : IEvent
    {
        public AdvLineChangedEvent(AdvLine line)
        {
            Line = line;
        }

        public AdvLine Line { get; }
    }
}