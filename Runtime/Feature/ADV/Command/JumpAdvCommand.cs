using MyArchitecture.Core;

namespace MyArchitecture.Feature.ADV
{
    public sealed class JumpAdvCommand : Command<string>
    {
        private readonly AdvScenarioService _scenarioService;

        public JumpAdvCommand(AdvScenarioService scenarioService)
        {
            _scenarioService = scenarioService;
        }

        protected override void OnExecute(string label)
        {
            _scenarioService.Jump(label);
        }
    }
}