using Cysharp.Threading.Tasks;
using MyArchitecture.Core;
using MyArchitecture.Integration;
using VContainer;

namespace MyArchitecture.Feature.Rhythm
{
    public abstract class RhythmInputPresenter : Presenter
    {
        private IRhythmInputView _view;

        [Inject]
        private void InjectRhythmInputPresenterDependencies(
            IRhythmInputView view)
        {
            _view = view;
        }

        protected override void OnBind()
        {
            this.SubscribeViewSignal(
                _view.InputSubmitted,
                input => this.SendCommand<SubmitRhythmInputCommand, RhythmInput>(input));

            this.SubscribeViewSignal(
                _view.PauseRequested,
                () => this.SendCommand<PauseRhythmCommand>());

            this.SubscribeViewSignal(
                _view.ResumeRequested,
                () => this.SendCommand<ResumeRhythmCommand>());

            this.SubscribeViewSignal(
                _view.StopRequested,
                () => this.SendCommand<StopRhythmCommand>());
        }

        protected UniTask StartChartAsync(string chartId)
        {
            return this.SendCommandAsync<StartRhythmChartCommand, string>(chartId);
        }
    }
}
