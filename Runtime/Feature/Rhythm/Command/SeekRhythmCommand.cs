using MyArchitecture.Core;

namespace MyArchitecture.Feature.Rhythm
{
    public sealed class SeekRhythmCommand : Command<double>
    {
        private readonly RhythmGameplayService _gameplayService;

        public SeekRhythmCommand(RhythmGameplayService gameplayService)
        {
            _gameplayService = gameplayService;
        }

        protected override void OnExecute(double chartTime)
        {
            _gameplayService.Seek(chartTime);
        }
    }
}
