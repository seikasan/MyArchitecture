using MyArchitecture.Core;

namespace MyArchitecture.Feature.ADV
{
    public sealed class AdvInstructionContext
    {
        public AdvInstructionContext(
            AdvScenarioModel scenarioModel,
            AdvStateModel stateModel,
            IEventPublisher eventPublisher)
        {
            ScenarioModel = scenarioModel;
            StateModel = stateModel;
            EventPublisher = eventPublisher;
        }

        public AdvScenarioModel ScenarioModel { get; }
        public AdvStateModel StateModel { get; }
        public IEventPublisher EventPublisher { get; }
    }
}