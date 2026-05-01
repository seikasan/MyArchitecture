using MyArchitecture.Core;

namespace MyArchitecture.Feature.Rhythm
{
    public interface IRhythmComboView : IView
    {
        void ShowCombo(
            int combo,
            int maxCombo);
    }
}
