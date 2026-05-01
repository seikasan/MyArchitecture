using System;

namespace MyArchitecture.Feature.Rhythm
{
    public readonly struct RhythmInput
    {
        public RhythmInput(
            int laneId,
            RhythmInputKind kind,
            double dspTime = 0d,
            string customKind = null)
        {
            LaneId = laneId;
            Kind = kind;
            DspTime = dspTime;
            CustomKind = customKind ?? string.Empty;
        }

        public int LaneId { get; }
        public RhythmInputKind Kind { get; }
        public double DspTime { get; }
        public string CustomKind { get; }
        public bool HasDspTime => DspTime > 0d;

        public RhythmInput WithDspTime(double dspTime)
        {
            return new RhythmInput(
                LaneId,
                Kind,
                dspTime,
                CustomKind);
        }

        public static RhythmInput Press(int laneId)
        {
            return new RhythmInput(laneId, RhythmInputKind.Press);
        }

        public static RhythmInput Release(int laneId)
        {
            return new RhythmInput(laneId, RhythmInputKind.Release);
        }

        public static RhythmInput Flick(int laneId)
        {
            return new RhythmInput(laneId, RhythmInputKind.Flick);
        }
    }

    public readonly struct RhythmNoteViewData
    {
        public RhythmNoteViewData(
            RhythmNote note,
            double scheduledDspStartTime,
            double chartOffset)
        {
            Note = note ?? throw new ArgumentNullException(nameof(note));
            ScheduledDspStartTime = scheduledDspStartTime;
            ChartOffset = chartOffset;
        }

        public RhythmNote Note { get; }
        public double ScheduledDspStartTime { get; }
        public double ChartOffset { get; }
    }

    public readonly struct RhythmJudgeResult
    {
        public RhythmJudgeResult(
            RhythmJudgeRank rank,
            RhythmNote note,
            RhythmInput input,
            double timingError)
        {
            Rank = rank;
            Note = note;
            NoteId = note != null ? note.NoteId : default;
            Input = input;
            TimingError = timingError;
        }

        public RhythmJudgeRank Rank { get; }
        public RhythmNote Note { get; }
        public RhythmNoteId NoteId { get; }
        public RhythmInput Input { get; }
        public double TimingError { get; }
        public bool IsJudged => Rank != RhythmJudgeRank.None;
        public bool IsHit => Rank != RhythmJudgeRank.None && Rank != RhythmJudgeRank.Miss;

        public static RhythmJudgeResult None(RhythmInput input)
        {
            return new RhythmJudgeResult(
                RhythmJudgeRank.None,
                null,
                input,
                0d);
        }

        public static RhythmJudgeResult Miss(
            RhythmNote note,
            double timingError)
        {
            return new RhythmJudgeResult(
                RhythmJudgeRank.Miss,
                note,
                default,
                timingError);
        }
    }

    public readonly struct RhythmScoreSnapshot
    {
        public RhythmScoreSnapshot(
            int score,
            int combo,
            int maxCombo,
            double accuracy,
            double gauge,
            int perfectCount,
            int greatCount,
            int goodCount,
            int badCount,
            int missCount)
        {
            Score = score;
            Combo = combo;
            MaxCombo = maxCombo;
            Accuracy = accuracy;
            Gauge = gauge;
            PerfectCount = perfectCount;
            GreatCount = greatCount;
            GoodCount = goodCount;
            BadCount = badCount;
            MissCount = missCount;
        }

        public int Score { get; }
        public int Combo { get; }
        public int MaxCombo { get; }
        public double Accuracy { get; }
        public double Gauge { get; }
        public int PerfectCount { get; }
        public int GreatCount { get; }
        public int GoodCount { get; }
        public int BadCount { get; }
        public int MissCount { get; }
        public int TotalJudged => PerfectCount + GreatCount + GoodCount + BadCount + MissCount;

        public static RhythmScoreSnapshot Empty()
        {
            return new RhythmScoreSnapshot(
                0,
                0,
                0,
                0d,
                0d,
                0,
                0,
                0,
                0,
                0);
        }
    }
}
