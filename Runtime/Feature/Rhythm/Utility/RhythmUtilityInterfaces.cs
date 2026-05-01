using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using MyArchitecture.Core;

namespace MyArchitecture.Feature.Rhythm
{
    public interface IRhythmChartProvider : IUtility
    {
        bool CanProvide(string chartId);

        UniTask<RhythmChart> LoadAsync(
            string chartId,
            CancellationToken cancellationToken);
    }

    public interface IRhythmClock : IUtility
    {
        double DspTime { get; }
    }

    public interface IRhythmJudgeEvaluator : IUtility
    {
        RhythmJudgeResult Evaluate(RhythmJudgeRequest request);
    }

    public interface IRhythmScoreCalculator : IUtility
    {
        RhythmScoreSnapshot Apply(
            RhythmScoreSnapshot current,
            RhythmJudgeResult result);
    }

    public readonly struct RhythmJudgeRequest
    {
        public RhythmJudgeRequest(
            RhythmInput input,
            double chartTime,
            IReadOnlyList<RhythmNote> candidateNotes,
            RhythmJudgeProfile judgeProfile)
        {
            Input = input;
            ChartTime = chartTime;
            CandidateNotes = candidateNotes;
            JudgeProfile = judgeProfile;
        }

        public RhythmInput Input { get; }
        public double ChartTime { get; }
        public IReadOnlyList<RhythmNote> CandidateNotes { get; }
        public RhythmJudgeProfile JudgeProfile { get; }
    }
}
