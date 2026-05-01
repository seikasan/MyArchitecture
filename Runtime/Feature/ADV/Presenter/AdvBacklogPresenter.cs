using MyArchitecture.Core;
using VContainer;

namespace MyArchitecture.Feature.ADV
{
    public abstract class AdvBacklogPresenter : Presenter
    {
        private IAdvBacklogView _view;
        private IReadOnlyAdvScenarioModel _scenarioModel;

        [Inject]
        private void InjectAdvBacklogPresenterDependencies(
            IAdvBacklogView view,
            IReadOnlyAdvScenarioModel scenarioModel)
        {
            _view = view;
            _scenarioModel = scenarioModel;
        }

        protected override void OnBind()
        {
            this.SubscribeEvent<AdvLineChangedEvent>(_ => RefreshBacklog());
            this.SubscribeEvent<AdvChoicesChangedEvent>(_ => RefreshBacklog());
            this.SubscribeEvent<AdvLoadCompletedEvent>(_ => RefreshBacklog());
        }

        protected void RefreshBacklog()
        {
            _view.ShowBacklog(_scenarioModel.Backlog);
        }

        protected void HideBacklog()
        {
            _view.HideBacklog();
        }
    }
}
