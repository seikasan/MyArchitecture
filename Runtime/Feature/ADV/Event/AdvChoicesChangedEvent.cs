using MyArchitecture.Core;

namespace MyArchitecture.Feature.ADV
{
    public readonly struct AdvChoicesChangedEvent : IEvent
    {
        public AdvChoicesChangedEvent(AdvChoice[] choices)
        {
            Choices = choices;
        }

        public AdvChoice[] Choices { get; }
    }
}