using System.Threading;
using Cysharp.Threading.Tasks;
using MyArchitecture.Core;

namespace MyArchitecture.Feature.ADV
{
    public sealed class StartAdvScenarioCommand : AsyncCommand<string>
    {
        private readonly AdvScenarioService _scenarioService;

        public StartAdvScenarioCommand(AdvScenarioService scenarioService)
        {
            _scenarioService = scenarioService;
        }

        protected override UniTask OnExecuteAsync(
            string scenarioId,
            CancellationToken cancellationToken)
        {
            return _scenarioService.StartAsync(
                scenarioId,
                cancellationToken);
        }
    }
}