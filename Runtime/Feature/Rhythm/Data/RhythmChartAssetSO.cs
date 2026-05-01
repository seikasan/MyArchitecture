using System;
using System.Linq;
using UnityEngine;

namespace MyArchitecture.Feature.Rhythm
{
    public sealed class RhythmChartAssetSO : ScriptableObject
    {
        [SerializeField] private RhythmChartData chart;

        public RhythmChart ToChart()
        {
            if (chart == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(RhythmChartAssetSO)} has no chart data.");
            }

            return chart.ToChart();
        }
    }

    [Serializable]
    public sealed class RhythmChartData
    {
        public string chartId;
        public string musicKey;
        public float chartOffset;
        public float length;
        public float lookAheadTime = 2f;
        public float despawnDelay = 0.25f;
        public RhythmNoteData[] notes;
        public RhythmTimingPointData[] timingPoints;
        public RhythmJudgeProfileData judgeProfile;

        public RhythmChart ToChart()
        {
            return new RhythmChart(
                chartId,
                musicKey,
                (notes ?? Array.Empty<RhythmNoteData>())
                .Select((note, index) => note.ToNote(index)),
                (timingPoints ?? Array.Empty<RhythmTimingPointData>())
                .Select(point => point.ToTimingPoint()),
                judgeProfile != null ? judgeProfile.ToJudgeProfile() : null,
                chartOffset,
                length,
                lookAheadTime,
                despawnDelay);
        }
    }

    [Serializable]
    public sealed class RhythmNoteData
    {
        public string noteId;
        public string kind;
        public int laneId;
        public float hitTime;
        public float duration;
        public int endLaneId;
        public string customKind;
        public string[] tags;

        public RhythmNote ToNote(int index)
        {
            string id = string.IsNullOrWhiteSpace(noteId)
                ? $"note_{index}"
                : noteId;

            return new RhythmNote(
                new RhythmNoteId(id),
                ParseKind(kind),
                laneId,
                hitTime,
                duration,
                endLaneId,
                customKind,
                tags);
        }

        private static RhythmNoteKind ParseKind(string value)
        {
            string normalized = (value ?? string.Empty)
                .Trim()
                .ToLowerInvariant();

            switch (normalized)
            {
                case "":
                case "tap":
                    return RhythmNoteKind.Tap;
                case "hold":
                    return RhythmNoteKind.Hold;
                case "flick":
                    return RhythmNoteKind.Flick;
                case "slide":
                    return RhythmNoteKind.Slide;
                case "custom":
                    return RhythmNoteKind.Custom;
                default:
                    throw new InvalidOperationException(
                        $"Rhythm note kind is unknown: {value}");
            }
        }
    }

    [Serializable]
    public sealed class RhythmTimingPointData
    {
        public float time;
        public float bpm = 120f;
        public int beatsPerMeasure = 4;

        public RhythmTimingPoint ToTimingPoint()
        {
            return new RhythmTimingPoint(
                time,
                bpm,
                beatsPerMeasure);
        }
    }

    [Serializable]
    public sealed class RhythmJudgeProfileData
    {
        public float perfectWindow = 0.033f;
        public float greatWindow = 0.066f;
        public float goodWindow = 0.1f;
        public float badWindow = 0.15f;
        public float missWindow = 0.18f;

        public RhythmJudgeProfile ToJudgeProfile()
        {
            return new RhythmJudgeProfile(
                perfectWindow,
                greatWindow,
                goodWindow,
                badWindow,
                missWindow);
        }
    }
}
