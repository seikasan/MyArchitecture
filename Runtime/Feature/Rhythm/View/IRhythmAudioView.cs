using MyArchitecture.Core;

namespace MyArchitecture.Feature.Rhythm
{
    public interface IRhythmAudioView : IView
    {
        void ScheduleMusic(
            string musicKey,
            double scheduledDspStartTime,
            double chartOffset);

        void PauseMusic();
        void ResumeMusic(double scheduledDspStartTime);
        void StopMusic();
    }
}
