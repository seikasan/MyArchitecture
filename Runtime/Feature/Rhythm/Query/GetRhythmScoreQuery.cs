using MyArchitecture.Core;

namespace MyArchitecture.Feature.Rhythm
{
    public sealed class GetRhythmScoreQuery :
        Query<RhythmScoreSnapshot>
    {
        private readonly IReadOnlyRhythmScoreModel _scoreModel;

        public GetRhythmScoreQuery(
            IReadOnlyRhythmScoreModel scoreModel)
        {
            _scoreModel = scoreModel;
        }

        protected override RhythmScoreSnapshot OnExecute()
        {
            return new RhythmScoreSnapshot(
                _scoreModel.Score,
                _scoreModel.Combo,
                _scoreModel.MaxCombo,
                _scoreModel.Accuracy,
                _scoreModel.Gauge,
                _scoreModel.PerfectCount,
                _scoreModel.GreatCount,
                _scoreModel.GoodCount,
                _scoreModel.BadCount,
                _scoreModel.MissCount);
        }
    }
}
