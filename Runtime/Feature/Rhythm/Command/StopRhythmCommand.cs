using MyArchitecture.Core;

namespace MyArchitecture.Feature.Rhythm
{
    public sealed class StopRhythmCommand : Command
    {
        private readonly RhythmGameplayService _gameplayService;

        public StopRhythmCommand(RhythmGameplayService gameplayService)
        {
            _gameplayService = gameplayService;
        }

        protected override void OnExecute()
        {
            _gameplayService.Stop();
        }
    }
}
