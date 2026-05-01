using MyArchitecture.Core;
using VContainer;

namespace MyArchitecture.Feature.Rhythm
{
    public abstract class RhythmComboPresenter : Presenter
    {
        private IRhythmComboView _view;
        private IReadOnlyRhythmScoreModel _scoreModel;

        [Inject]
        private void InjectRhythmComboPresenterDependencies(
            IRhythmComboView view,
            IReadOnlyRhythmScoreModel scoreModel)
        {
            _view = view;
            _scoreModel = scoreModel;
        }

        protected override void OnBind()
        {
            this.SubscribeEvent<RhythmComboChangedEvent>(
                eventData => _view.ShowCombo(
                    eventData.Combo,
                    eventData.MaxCombo));
            this.SubscribeEvent<RhythmStartedEvent>(_ => RefreshCombo());
        }

        protected virtual void RefreshCombo()
        {
            _view.ShowCombo(
                _scoreModel.Combo,
                _scoreModel.MaxCombo);
        }
    }
}
