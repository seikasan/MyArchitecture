using MyArchitecture.Core;
using VContainer;

namespace MyArchitecture.Feature.ADV
{
    public abstract class AdvDialoguePresenter : Presenter
    {
        private IAdvDialogueView _view;
        private IReadOnlyAdvScenarioModel _scenarioModel;

        [Inject]
        private void InjectAdvDialoguePresenterDependencies(
            IAdvDialogueView view,
            IReadOnlyAdvScenarioModel scenarioModel)
        {
            _view = view;
            _scenarioModel = scenarioModel;
        }

        protected override void OnBind()
        {
            this.SubscribeEvent<AdvLineChangedEvent>(OnLineChanged);
            this.SubscribeEvent<AdvScenarioEndedEvent>(_ => _view.ClearLine());
            this.SubscribeEvent<AdvLoadCompletedEvent>(_ => RefreshLineFromModel());
        }

        protected virtual void OnLineChanged(AdvLineChangedEvent eventData)
        {
            _view.ShowLine(eventData.Line);
        }

        protected virtual void RefreshLineFromModel()
        {
            if (_scenarioModel.PlaybackState == AdvPlaybackState.Ended ||
                _scenarioModel.CurrentLine == null)
            {
                _view.ClearLine();
                return;
            }

            _view.ShowLine(_scenarioModel.CurrentLine);
        }
    }
}
