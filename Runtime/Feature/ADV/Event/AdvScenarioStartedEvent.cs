using MyArchitecture.Core;

namespace MyArchitecture.Feature.ADV
{
    public readonly struct AdvScenarioStartedEvent : IEvent
    {
        public AdvScenarioStartedEvent(string scenarioId)
        {
            ScenarioId = scenarioId;
        }

        public string ScenarioId { get; }
    }
}