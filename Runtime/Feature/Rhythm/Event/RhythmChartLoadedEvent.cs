using MyArchitecture.Core;

namespace MyArchitecture.Feature.Rhythm
{
    public readonly struct RhythmChartLoadedEvent : IEvent
    {
        public RhythmChartLoadedEvent(RhythmChart chart)
        {
            Chart = chart;
        }

        public RhythmChart Chart { get; }
    }
}
