using MyArchitecture.Core;

namespace MyArchitecture.Feature.Rhythm
{
    public readonly struct RhythmPausedEvent : IEvent
    {
        public RhythmPausedEvent(
            string chartId,
            double chartTime)
        {
            ChartId = chartId;
            ChartTime = chartTime;
        }

        public string ChartId { get; }
        public double ChartTime { get; }
    }
}
