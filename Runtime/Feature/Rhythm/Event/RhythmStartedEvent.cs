using MyArchitecture.Core;

namespace MyArchitecture.Feature.Rhythm
{
    public readonly struct RhythmStartedEvent : IEvent
    {
        public RhythmStartedEvent(string chartId)
        {
            ChartId = chartId;
        }

        public string ChartId { get; }
    }
}
