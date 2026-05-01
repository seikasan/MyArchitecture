using MyArchitecture.Core;

namespace MyArchitecture.Feature.Rhythm
{
    public interface IRhythmProgressView : IView
    {
        void ShowProgress(
            double chartTime,
            double chartLength,
            RhythmPlaybackState playbackState);
    }
}
