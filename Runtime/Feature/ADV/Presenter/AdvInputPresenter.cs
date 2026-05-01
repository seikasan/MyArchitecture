using Cysharp.Threading.Tasks;
using MyArchitecture.Core;
using R3;
using VContainer;

namespace MyArchitecture.Feature.ADV
{
      public abstract class AdvInputPresenter : Presenter
    {
        private IAdvInputView _view;
        private IReadOnlyAdvScenarioModel _scenarioModel;

        [Inject]
        private void InjectAdvInputPresenterDependencies(
            IAdvInputView view,
            IReadOnlyAdvScenarioModel scenarioModel)
        {
            _view = view;
            _scenarioModel = scenarioModel;
        }

        protected override void OnBind()
        {
            Track(_view.AdvanceRequested.AsObservable()
                .Subscribe(_ => this.SendCommand<AdvanceAdvCommand>()));

            Track(_view.SaveRequested.AsObservable()
                .Subscribe(_ => this.SendCommandAsync<SaveAdvCommand>().Forget()));

            Track(_view.LoadRequested.AsObservable()
                .Subscribe(_ => this.SendCommandAsync<LoadAdvCommand>().Forget()));

            Track(_view.SkipRequested.AsObservable()
                .Subscribe(_ => OnSkipRequested()));

            Track(_view.AutoRequested.AsObservable()
                .Subscribe(_ => OnAutoRequested()));

            Track(_view.BacklogRequested.AsObservable()
                .Subscribe(_ => OnBacklogRequested()));

            this.SubscribeEvent<AdvLineChangedEvent>(_ => RefreshInputState());
            this.SubscribeEvent<AdvChoicesChangedEvent>(_ => RefreshInputState());
            this.SubscribeEvent<AdvWaitStartedEvent>(_ => RefreshInputState());
            this.SubscribeEvent<AdvScenarioEndedEvent>(_ => RefreshInputState());
            this.SubscribeEvent<AdvLoadCompletedEvent>(_ => RefreshInputState());
        }

        protected virtual void RefreshInputState()
        {
            _view.SetAdvanceEnabled(
                _scenarioModel.PlaybackState == AdvPlaybackState.WaitingForAdvance);
        }

        protected virtual void OnSkipRequested()
        {
        }

        protected virtual void OnAutoRequested()
        {
        }

        protected virtual void OnBacklogRequested()
        {
        }
    }
}
