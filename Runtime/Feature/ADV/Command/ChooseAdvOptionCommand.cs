using MyArchitecture.Core;

namespace MyArchitecture.Feature.ADV
{
    public sealed class ChooseAdvOptionCommand : Command<string>
    {
        private readonly AdvScenarioService _scenarioService;

        public ChooseAdvOptionCommand(AdvScenarioService scenarioService)
        {
            _scenarioService = scenarioService;
        }

        protected override void OnExecute(string choiceId)
        {
            _scenarioService.Choose(choiceId);
        }
    }
}