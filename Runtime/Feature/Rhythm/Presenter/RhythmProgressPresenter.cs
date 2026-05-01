using MyArchitecture.Core;
using VContainer;

namespace MyArchitecture.Feature.Rhythm
{
    public abstract class RhythmProgressPresenter : Presenter
    {
        private IRhythmProgressView _view;
        private IReadOnlyRhythmPlaybackModel _playbackModel;

        [Inject]
        private void InjectRhythmProgressPresenterDependencies(
            IRhythmProgressView view,
            IReadOnlyRhythmPlaybackModel playbackModel)
        {
            _view = view;
            _playbackModel = playbackModel;
        }

        protected override void OnBind()
        {
            this.SubscribeEvent<RhythmStartedEvent>(_ => RefreshProgress());
            this.SubscribeEvent<RhythmPausedEvent>(_ => RefreshProgress());
            this.SubscribeEvent<RhythmResumedEvent>(_ => RefreshProgress());
            this.SubscribeEvent<RhythmStoppedEvent>(_ => RefreshProgress());
            this.SubscribeEvent<RhythmEndedEvent>(_ => RefreshProgress());
        }

        protected virtual void RefreshProgress()
        {
            _view.ShowProgress(
                _playbackModel.ChartTime,
                _playbackModel.CurrentChart != null
                    ? _playbackModel.CurrentChart.Length
                    : 0d,
                _playbackModel.PlaybackState);
        }
    }
}
