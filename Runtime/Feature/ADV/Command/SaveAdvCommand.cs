using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using MyArchitecture.Core;

namespace MyArchitecture.Feature.ADV
{
        public sealed class SaveAdvCommand : AsyncCommand
    {
        private readonly AdvScenarioModel _scenarioModel;
        private readonly AdvStateModel _stateModel;
        private readonly IAdvSaveDataMapper _saveDataMapper;
        private readonly ISaveDataRepository<AdvSaveData> _repository;

        public SaveAdvCommand(
            AdvScenarioModel scenarioModel,
            AdvStateModel stateModel,
            IEnumerable<IAdvSaveDataMapper> saveDataMappers,
            ISaveDataRepository<AdvSaveData> repository)
        {
            _scenarioModel = scenarioModel;
            _stateModel = stateModel;
            _saveDataMapper = SelectSaveDataMapper(saveDataMappers);
            _repository = repository;
        }

        protected override async UniTask OnExecuteAsync(
            CancellationToken cancellationToken)
        {
            AdvSaveData saveData = _saveDataMapper.CreateSaveData(
                _scenarioModel.CreateSnapshot(),
                _stateModel.CreateSnapshot());

            await _repository.SaveAsync(
                saveData,
                cancellationToken);

            this.PublishEvent(new AdvSaveCompletedEvent(saveData));
        }

        private static IAdvSaveDataMapper SelectSaveDataMapper(
            IEnumerable<IAdvSaveDataMapper> saveDataMappers)
        {
            IAdvSaveDataMapper defaultMapper = null;
            IAdvSaveDataMapper customMapper = null;

            foreach (var mapper in saveDataMappers ?? Array.Empty<IAdvSaveDataMapper>())
            {
                if (mapper is DefaultAdvSaveDataMapper)
                {
                    defaultMapper ??= mapper;
                    continue;
                }

                if (customMapper != null)
                {
                    throw new InvalidOperationException(
                        "Multiple custom ADV save data mappers are registered.");
                }

                customMapper = mapper;
            }

            return customMapper ?? defaultMapper ?? new DefaultAdvSaveDataMapper();
        }
    }
}