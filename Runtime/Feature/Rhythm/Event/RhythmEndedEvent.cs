using MyArchitecture.Core;

namespace MyArchitecture.Feature.Rhythm
{
    public readonly struct RhythmEndedEvent : IEvent
    {
        public RhythmEndedEvent(
            string chartId,
            RhythmScoreSnapshot score)
        {
            ChartId = chartId;
            Score = score;
        }

        public string ChartId { get; }
        public RhythmScoreSnapshot Score { get; }
    }
}
