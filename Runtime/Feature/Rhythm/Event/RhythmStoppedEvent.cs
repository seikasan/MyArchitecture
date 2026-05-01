using MyArchitecture.Core;

namespace MyArchitecture.Feature.Rhythm
{
    public readonly struct RhythmStoppedEvent : IEvent
    {
        public RhythmStoppedEvent(string chartId)
        {
            ChartId = chartId;
        }

        public string ChartId { get; }
    }
}
