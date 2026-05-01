using MyArchitecture.Core;

namespace MyArchitecture.Feature.Rhythm
{
    public readonly struct RhythmInputJudgedEvent : IEvent
    {
        public RhythmInputJudgedEvent(RhythmJudgeResult result)
        {
            Result = result;
        }

        public RhythmJudgeResult Result { get; }
    }
}
