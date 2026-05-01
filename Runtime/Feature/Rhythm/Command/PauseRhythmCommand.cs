using MyArchitecture.Core;

namespace MyArchitecture.Feature.Rhythm
{
    public sealed class PauseRhythmCommand : Command
    {
        private readonly RhythmGameplayService _gameplayService;

        public PauseRhythmCommand(RhythmGameplayService gameplayService)
        {
            _gameplayService = gameplayService;
        }

        protected override void OnExecute()
        {
            _gameplayService.Pause();
        }
    }
}
