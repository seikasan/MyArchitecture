using MyArchitecture.Core;

namespace MyArchitecture.Feature.Rhythm
{
    public readonly struct RhythmPlayScheduledEvent : IEvent
    {
        public RhythmPlayScheduledEvent(
            string chartId,
            string musicKey,
            double scheduledDspStartTime,
            double chartOffset)
        {
            ChartId = chartId;
            MusicKey = musicKey;
            ScheduledDspStartTime = scheduledDspStartTime;
            ChartOffset = chartOffset;
        }

        public string ChartId { get; }
        public string MusicKey { get; }
        public double ScheduledDspStartTime { get; }
        public double ChartOffset { get; }
    }
}
