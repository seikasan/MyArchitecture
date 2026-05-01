using MyArchitecture.Core;
using R3;
using VContainer;

namespace MyArchitecture.Feature.ADV
{
    public abstract class AdvChoicePresenter : Presenter
    {
        private IAdvChoiceView _view;
        private IReadOnlyAdvScenarioModel _scenarioModel;

        [Inject]
        private void InjectAdvChoicePresenterDependencies(
            IAdvChoiceView view,
            IReadOnlyAdvScenarioModel scenarioModel)
        {
            _view = view;
            _scenarioModel = scenarioModel;
        }

        protected override void OnBind()
        {
            Track(_view.ChoiceSelected.AsObservable()
                .Subscribe(this.SendCommand<ChooseAdvOptionCommand, string>));

            this.SubscribeEvent<AdvChoicesChangedEvent>(OnChoicesChanged);
            this.SubscribeEvent<AdvLineChangedEvent>(_ => _view.ClearChoices());
            this.SubscribeEvent<AdvScenarioEndedEvent>(_ => _view.ClearChoices());
            this.SubscribeEvent<AdvLoadCompletedEvent>(_ => RefreshChoicesFromModel());
        }

        protected virtual void OnChoicesChanged(AdvChoicesChangedEvent eventData)
        {
            if (eventData.Choices == null ||
                eventData.Choices.Length == 0)
            {
                _view.ClearChoices();
                return;
            }

            _view.ShowChoices(eventData.Choices);
        }

        protected virtual void RefreshChoicesFromModel()
        {
            if (_scenarioModel.PlaybackState != AdvPlaybackState.WaitingForChoice ||
                _scenarioModel.CurrentChoices == null ||
                _scenarioModel.CurrentChoices.Count == 0)
            {
                _view.ClearChoices();
                return;
            }

            _view.ShowChoices(_scenarioModel.CurrentChoices);
        }
    }
}
