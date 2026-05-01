using MyArchitecture.Core;

namespace MyArchitecture.Feature.Rhythm
{
    public readonly struct RhythmComboChangedEvent : IEvent
    {
        public RhythmComboChangedEvent(
            int combo,
            int maxCombo)
        {
            Combo = combo;
            MaxCombo = maxCombo;
        }

        public int Combo { get; }
        public int MaxCombo { get; }
    }
}
