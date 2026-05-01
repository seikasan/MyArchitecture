using System.Threading;
using Cysharp.Threading.Tasks;
using MyArchitecture.Core;

namespace MyArchitecture.Feature.ADV
{
    public sealed class LoadAdvCommand : AsyncCommand
    {
        private readonly AdvScenarioService _scenarioService;
        private readonly ISaveDataRepository<AdvSaveData> _repository;

        public LoadAdvCommand(
            AdvScenarioService scenarioService,
            ISaveDataRepository<AdvSaveData> repository)
        {
            _scenarioService = scenarioService;
            _repository = repository;
        }

        protected override async UniTask OnExecuteAsync(
            CancellationToken cancellationToken)
        {
            AdvSaveData saveData = await _repository.LoadAsync(
                cancellationToken);

            await _scenarioService.RestoreAsync(
                saveData,
                cancellationToken);
        }
    }
}