using System.Threading;
using Cysharp.Threading.Tasks;
using MyArchitecture.Core;

namespace MyArchitecture.Feature.Rhythm
{
    public sealed class StartRhythmChartCommand : AsyncCommand<string>
    {
        private readonly RhythmGameplayService _gameplayService;

        public StartRhythmChartCommand(RhythmGameplayService gameplayService)
        {
            _gameplayService = gameplayService;
        }

        protected override UniTask OnExecuteAsync(
            string chartId,
            CancellationToken cancellationToken)
        {
            return _gameplayService.StartAsync(
                chartId,
                cancellationToken);
        }
    }
}
