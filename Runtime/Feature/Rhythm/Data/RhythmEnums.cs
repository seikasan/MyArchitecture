namespace MyArchitecture.Feature.Rhythm
{
    public enum RhythmNoteKind
    {
        Tap,
        Hold,
        Flick,
        Slide,
        Custom
    }

    public enum RhythmInputKind
    {
        Press,
        Release,
        Flick,
        Custom
    }

    public enum RhythmPlaybackState
    {
        Stopped,
        Playing,
        Paused,
        Ended
    }

    public enum RhythmJudgeRank
    {
        None,
        Perfect,
        Great,
        Good,
        Bad,
        Miss
    }
}
