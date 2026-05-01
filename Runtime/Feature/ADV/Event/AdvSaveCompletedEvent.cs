using MyArchitecture.Core;

namespace MyArchitecture.Feature.ADV
{
    public readonly struct AdvSaveCompletedEvent : IEvent
    {
        public AdvSaveCompletedEvent(AdvSaveData saveData)
        {
            SaveData = saveData;
        }

        public AdvSaveData SaveData { get; }
    }
}