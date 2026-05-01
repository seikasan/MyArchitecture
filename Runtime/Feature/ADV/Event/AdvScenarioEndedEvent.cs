using MyArchitecture.Core;

namespace MyArchitecture.Feature.ADV
{
    public readonly struct AdvScenarioEndedEvent : IEvent
    {
        public AdvScenarioEndedEvent(string scenarioId)
        {
            ScenarioId = scenarioId;
        }

        public string ScenarioId { get; }
    }
}