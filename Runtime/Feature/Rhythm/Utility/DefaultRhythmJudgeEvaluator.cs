using System;
using MyArchitecture.Core;

namespace MyArchitecture.Feature.Rhythm
{
    public sealed class DefaultRhythmJudgeEvaluator :
        Utility,
        IRhythmJudgeEvaluator
    {
        public RhythmJudgeResult Evaluate(RhythmJudgeRequest request)
        {
            var profile = request.JudgeProfile ?? RhythmJudgeProfile.Default();
            RhythmNote bestNote = null;
            double bestError = 0d;
            double bestAbsError = double.MaxValue;

            if (request.CandidateNotes == null)
            {
                return RhythmJudgeResult.None(request.Input);
            }

            foreach (var note in request.CandidateNotes)
            {
                if (note == null ||
                    !CanJudge(note, request.Input))
                {
                    continue;
                }

                double targetTime = GetTargetTime(note, request.Input);
                double error = request.ChartTime - targetTime;
                double absError = Math.Abs(error);

                if (absError > profile.MissWindow ||
                    absError >= bestAbsError)
                {
                    continue;
                }

                bestNote = note;
                bestError = error;
                bestAbsError = absError;
            }

            if (bestNote == null)
            {
                return RhythmJudgeResult.None(request.Input);
            }

            return new RhythmJudgeResult(
                GetRank(bestAbsError, profile),
                bestNote,
                request.Input,
                bestError);
        }

        private static bool CanJudge(
            RhythmNote note,
            RhythmInput input)
        {
            if (input.Kind == RhythmInputKind.Release)
            {
                return note.Kind == RhythmNoteKind.Hold &&
                       input.LaneId == note.EndLaneId;
            }

            if (input.LaneId != note.LaneId)
            {
                return false;
            }

            switch (note.Kind)
            {
                case RhythmNoteKind.Tap:
                    return input.Kind == RhythmInputKind.Press;
                case RhythmNoteKind.Hold:
                    return input.Kind == RhythmInputKind.Press;
                case RhythmNoteKind.Flick:
                    return input.Kind == RhythmInputKind.Flick;
                case RhythmNoteKind.Slide:
                    return input.Kind == RhythmInputKind.Press ||
                           input.Kind == RhythmInputKind.Flick;
                case RhythmNoteKind.Custom:
                    return input.Kind == RhythmInputKind.Custom &&
                           (string.IsNullOrEmpty(note.CustomKind) ||
                            note.CustomKind == input.CustomKind);
                default:
                    return false;
            }
        }

        private static double GetTargetTime(
            RhythmNote note,
            RhythmInput input)
        {
            return input.Kind == RhythmInputKind.Release
                ? note.EndTime
                : note.HitTime;
        }

        private static RhythmJudgeRank GetRank(
            double absError,
            RhythmJudgeProfile profile)
        {
            if (absError <= profile.PerfectWindow) return RhythmJudgeRank.Perfect;
            if (absError <= profile.GreatWindow) return RhythmJudgeRank.Great;
            if (absError <= profile.GoodWindow) return RhythmJudgeRank.Good;
            if (absError <= profile.BadWindow) return RhythmJudgeRank.Bad;
            return RhythmJudgeRank.Miss;
        }
    }
}
