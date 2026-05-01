using MyArchitecture.Core;
using VContainer;

namespace MyArchitecture.Feature.Rhythm
{
    public abstract class RhythmAudioPresenter : Presenter
    {
        private IRhythmAudioView _view;

        [Inject]
        private void InjectRhythmAudioPresenterDependencies(
            IRhythmAudioView view)
        {
            _view = view;
        }

        protected override void OnBind()
        {
            this.SubscribeEvent<RhythmPlayScheduledEvent>(OnPlayScheduled);
            this.SubscribeEvent<RhythmPausedEvent>(_ => _view.PauseMusic());
            this.SubscribeEvent<RhythmResumedEvent>(OnResumed);
            this.SubscribeEvent<RhythmStoppedEvent>(_ => _view.StopMusic());
            this.SubscribeEvent<RhythmEndedEvent>(_ => _view.StopMusic());
        }

        protected virtual void OnPlayScheduled(RhythmPlayScheduledEvent eventData)
        {
            _view.ScheduleMusic(
                eventData.MusicKey,
                eventData.ScheduledDspStartTime,
                eventData.ChartOffset);
        }

        protected virtual void OnResumed(RhythmResumedEvent eventData)
        {
            _view.ResumeMusic(eventData.ScheduledDspStartTime);
        }
    }
}
