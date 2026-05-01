using System;
using MyArchitecture.Core;

namespace MyArchitecture.Feature.Rhythm
{
    public sealed class DefaultRhythmScoreCalculator :
        Utility,
        IRhythmScoreCalculator
    {
        public RhythmScoreSnapshot Apply(
            RhythmScoreSnapshot current,
            RhythmJudgeResult result)
        {
            if (!result.IsJudged)
            {
                return current;
            }

            int score = current.Score + GetScore(result.Rank);
            int perfect = current.PerfectCount;
            int great = current.GreatCount;
            int good = current.GoodCount;
            int bad = current.BadCount;
            int miss = current.MissCount;
            int combo = current.Combo;

            switch (result.Rank)
            {
                case RhythmJudgeRank.Perfect:
                    perfect++;
                    combo++;
                    break;
                case RhythmJudgeRank.Great:
                    great++;
                    combo++;
                    break;
                case RhythmJudgeRank.Good:
                    good++;
                    combo++;
                    break;
                case RhythmJudgeRank.Bad:
                    bad++;
                    combo = 0;
                    break;
                case RhythmJudgeRank.Miss:
                    miss++;
                    combo = 0;
                    break;
            }

            int total = perfect + great + good + bad + miss;
            double accuracy = total == 0
                ? 0d
                : ((perfect * 1d) + (great * 0.8d) + (good * 0.5d) + (bad * 0.2d)) / total;
            double gauge = Clamp01(
                current.Gauge + GetGaugeDelta(result.Rank));

            return new RhythmScoreSnapshot(
                score,
                combo,
                Math.Max(current.MaxCombo, combo),
                accuracy,
                gauge,
                perfect,
                great,
                good,
                bad,
                miss);
        }

        private static int GetScore(RhythmJudgeRank rank)
        {
            switch (rank)
            {
                case RhythmJudgeRank.Perfect:
                    return 1000;
                case RhythmJudgeRank.Great:
                    return 800;
                case RhythmJudgeRank.Good:
                    return 500;
                case RhythmJudgeRank.Bad:
                    return 100;
                default:
                    return 0;
            }
        }

        private static double GetGaugeDelta(RhythmJudgeRank rank)
        {
            switch (rank)
            {
                case RhythmJudgeRank.Perfect:
                    return 0.01d;
                case RhythmJudgeRank.Great:
                    return 0.008d;
                case RhythmJudgeRank.Good:
                    return 0.004d;
                case RhythmJudgeRank.Bad:
                    return -0.02d;
                case RhythmJudgeRank.Miss:
                    return -0.04d;
                default:
                    return 0d;
            }
        }

        private static double Clamp01(double value)
        {
            if (value < 0d) return 0d;
            if (value > 1d) return 1d;
            return value;
        }
    }
}
