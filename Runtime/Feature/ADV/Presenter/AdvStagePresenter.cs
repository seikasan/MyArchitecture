using MyArchitecture.Core;
using VContainer;

namespace MyArchitecture.Feature.ADV
{
    public abstract class AdvStagePresenter : Presenter
    {
        private IAdvStageView _view;

        [Inject]
        private void InjectAdvStagePresenterDependencies(
            IAdvStageView view)
        {
            _view = view;
        }

        protected override void OnBind()
        {
            this.SubscribeEvent<AdvSignalEvent>(OnSignal);
        }

        protected virtual void OnSignal(AdvSignalEvent eventData)
        {
            _view.ApplySignal(eventData.Signal);
        }
    }
}