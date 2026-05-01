using MyArchitecture.Core;

namespace MyArchitecture.Feature.ADV
{
    public sealed class ResumeAdvWaitCommand : Command
    {
        private readonly AdvScenarioService _scenarioService;

        public ResumeAdvWaitCommand(AdvScenarioService scenarioService)
        {
            _scenarioService = scenarioService;
        }

        protected override void OnExecute()
        {
            _scenarioService.ResumeWait();
        }
    }
}