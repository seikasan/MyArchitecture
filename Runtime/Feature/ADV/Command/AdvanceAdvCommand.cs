using MyArchitecture.Core;

namespace MyArchitecture.Feature.ADV
{
    public sealed class AdvanceAdvCommand : Command
    {
        private readonly AdvScenarioService _scenarioService;

        public AdvanceAdvCommand(AdvScenarioService scenarioService)
        {
            _scenarioService = scenarioService;
        }

        protected override void OnExecute()
        {
            _scenarioService.Advance();
        }
    }
}