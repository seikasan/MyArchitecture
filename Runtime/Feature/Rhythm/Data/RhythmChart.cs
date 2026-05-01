using System;
using System.Collections.Generic;
using System.Linq;

namespace MyArchitecture.Feature.Rhythm
{
    public sealed class RhythmChart
    {
        private readonly IReadOnlyList<RhythmNote> _notes;
        private readonly IReadOnlyList<RhythmTimingPoint> _timingPoints;

        public RhythmChart(
            string chartId,
            string musicKey,
            IEnumerable<RhythmNote> notes,
            IEnumerable<RhythmTimingPoint> timingPoints = null,
            RhythmJudgeProfile judgeProfile = null,
            double chartOffset = 0d,
            double length = 0d,
            double lookAheadTime = 2d,
            double despawnDelay = 0.25d)
        {
            if (string.IsNullOrWhiteSpace(chartId))
            {
                throw new ArgumentException(
                    "Rhythm chart id is null or empty.",
                    nameof(chartId));
            }

            ChartId = chartId;
            MusicKey = musicKey ?? string.Empty;
            ChartOffset = chartOffset;
            JudgeProfile = judgeProfile ?? RhythmJudgeProfile.Default();
            LookAheadTime = Math.Max(0d, lookAheadTime);
            DespawnDelay = Math.Max(0d, despawnDelay);
            _notes = (notes ?? Enumerable.Empty<RhythmNote>())
                .OrderBy(note => note.HitTime)
                .ToArray();
            _timingPoints = (timingPoints ?? Enumerable.Empty<RhythmTimingPoint>())
                .OrderBy(point => point.Time)
                .ToArray();

            double lastNoteTime = _notes.Count == 0
                ? 0d
                : _notes.Max(note => note.EndTime);

            Length = Math.Max(length, lastNoteTime);
        }

        public string ChartId { get; }
        public string MusicKey { get; }
        public double ChartOffset { get; }
        public double Length { get; }
        public double LookAheadTime { get; }
        public double DespawnDelay { get; }
        public RhythmJudgeProfile JudgeProfile { get; }
        public IReadOnlyList<RhythmNote> Notes => _notes;
        public IReadOnlyList<RhythmTimingPoint> TimingPoints => _timingPoints;
    }

    public sealed class RhythmNote
    {
        private readonly IReadOnlyList<string> _tags;

        public RhythmNote(
            RhythmNoteId noteId,
            RhythmNoteKind kind,
            int laneId,
            double hitTime,
            double duration = 0d,
            int? endLaneId = null,
            string customKind = null,
            IEnumerable<string> tags = null)
        {
            NoteId = noteId;
            Kind = kind;
            LaneId = laneId;
            HitTime = hitTime;
            Duration = Math.Max(0d, duration);
            EndLaneId = endLaneId ?? laneId;
            CustomKind = customKind ?? string.Empty;
            _tags = (tags ?? Enumerable.Empty<string>())
                .Where(tag => !string.IsNullOrWhiteSpace(tag))
                .ToArray();
        }

        public RhythmNoteId NoteId { get; }
        public RhythmNoteKind Kind { get; }
        public int LaneId { get; }
        public double HitTime { get; }
        public double Duration { get; }
        public int EndLaneId { get; }
        public string CustomKind { get; }
        public IReadOnlyList<string> Tags => _tags;
        public double EndTime => HitTime + Duration;
    }

    public sealed class RhythmTimingPoint
    {
        public RhythmTimingPoint(
            double time,
            double bpm,
            int beatsPerMeasure = 4)
        {
            Time = time;
            Bpm = bpm;
            BeatsPerMeasure = beatsPerMeasure <= 0 ? 4 : beatsPerMeasure;
        }

        public double Time { get; }
        public double Bpm { get; }
        public int BeatsPerMeasure { get; }
    }

    public sealed class RhythmJudgeProfile
    {
        public RhythmJudgeProfile(
            double perfectWindow = 0.033d,
            double greatWindow = 0.066d,
            double goodWindow = 0.1d,
            double badWindow = 0.15d,
            double missWindow = 0.18d)
        {
            PerfectWindow = Math.Max(0d, perfectWindow);
            GreatWindow = Math.Max(PerfectWindow, greatWindow);
            GoodWindow = Math.Max(GreatWindow, goodWindow);
            BadWindow = Math.Max(GoodWindow, badWindow);
            MissWindow = Math.Max(BadWindow, missWindow);
        }

        public double PerfectWindow { get; }
        public double GreatWindow { get; }
        public double GoodWindow { get; }
        public double BadWindow { get; }
        public double MissWindow { get; }

        public static RhythmJudgeProfile Default()
        {
            return new RhythmJudgeProfile();
        }
    }
}
