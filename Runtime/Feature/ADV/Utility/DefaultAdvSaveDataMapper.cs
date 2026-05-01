using MyArchitecture.Core;

namespace MyArchitecture.Feature.ADV
{
    public interface IAdvSaveDataMapper : IUtility
    {
        AdvSaveData CreateSaveData(
            AdvScenarioSnapshot scenario,
            AdvStateSnapshot state);
    }

    public sealed class DefaultAdvSaveDataMapper :
        Utility,
        IAdvSaveDataMapper
    {
        public AdvSaveData CreateSaveData(
            AdvScenarioSnapshot scenario,
            AdvStateSnapshot state)
        {
            return new AdvSaveData(scenario, state);
        }
    }
}