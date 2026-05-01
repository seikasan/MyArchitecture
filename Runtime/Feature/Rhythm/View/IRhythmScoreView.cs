using MyArchitecture.Core;

namespace MyArchitecture.Feature.Rhythm
{
    public interface IRhythmScoreView : IView
    {
        void ShowScore(RhythmScoreSnapshot score);
    }
}
