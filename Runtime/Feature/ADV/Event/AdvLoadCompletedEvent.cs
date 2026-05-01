using MyArchitecture.Core;

namespace MyArchitecture.Feature.ADV
{
    public readonly struct AdvLoadCompletedEvent : IEvent
    {
        public AdvLoadCompletedEvent(
            AdvSaveData saveData)
        {
            SaveData = saveData;
        }

        public AdvSaveData SaveData { get; }
    }
}