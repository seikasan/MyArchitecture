using MyArchitecture.Core;
using VContainer;

namespace MyArchitecture.Feature.Rhythm
{
    public abstract class RhythmScorePresenter : Presenter
    {
        private IRhythmScoreView _view;
        private IReadOnlyRhythmScoreModel _scoreModel;

        [Inject]
        private void InjectRhythmScorePresenterDependencies(
            IRhythmScoreView view,
            IReadOnlyRhythmScoreModel scoreModel)
        {
            _view = view;
            _scoreModel = scoreModel;
        }

        protected override void OnBind()
        {
            this.SubscribeEvent<RhythmScoreChangedEvent>(
                eventData => _view.ShowScore(eventData.Score));
            this.SubscribeEvent<RhythmStartedEvent>(_ => RefreshScore());
        }

        protected virtual void RefreshScore()
        {
            _view.ShowScore(
                new RhythmScoreSnapshot(
                    _scoreModel.Score,
                    _scoreModel.Combo,
                    _scoreModel.MaxCombo,
                    _scoreModel.Accuracy,
                    _scoreModel.Gauge,
                    _scoreModel.PerfectCount,
                    _scoreModel.GreatCount,
                    _scoreModel.GoodCount,
                    _scoreModel.BadCount,
                    _scoreModel.MissCount));
        }
    }
}
