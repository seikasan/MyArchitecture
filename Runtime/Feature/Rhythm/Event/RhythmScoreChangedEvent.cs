using MyArchitecture.Core;

namespace MyArchitecture.Feature.Rhythm
{
    public readonly struct RhythmScoreChangedEvent : IEvent
    {
        public RhythmScoreChangedEvent(RhythmScoreSnapshot score)
        {
            Score = score;
        }

        public RhythmScoreSnapshot Score { get; }
    }
}
