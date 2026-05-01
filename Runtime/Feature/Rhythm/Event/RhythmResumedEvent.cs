using MyArchitecture.Core;

namespace MyArchitecture.Feature.Rhythm
{
    public readonly struct RhythmResumedEvent : IEvent
    {
        public RhythmResumedEvent(
            string chartId,
            double chartTime,
            double scheduledDspStartTime)
        {
            ChartId = chartId;
            ChartTime = chartTime;
            ScheduledDspStartTime = scheduledDspStartTime;
        }

        public string ChartId { get; }
        public double ChartTime { get; }
        public double ScheduledDspStartTime { get; }
    }
}
