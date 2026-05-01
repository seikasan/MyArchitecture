using MyArchitecture.Core;

namespace MyArchitecture.Feature.Rhythm
{
    public sealed class ResumeRhythmCommand : Command
    {
        private readonly RhythmGameplayService _gameplayService;

        public ResumeRhythmCommand(RhythmGameplayService gameplayService)
        {
            _gameplayService = gameplayService;
        }

        protected override void OnExecute()
        {
            _gameplayService.Resume();
        }
    }
}
