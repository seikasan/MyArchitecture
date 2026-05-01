using MyArchitecture.Core;

namespace MyArchitecture.Feature.Rhythm
{
    public sealed class SubmitRhythmInputCommand : Command<RhythmInput>
    {
        private readonly RhythmGameplayService _gameplayService;

        public SubmitRhythmInputCommand(RhythmGameplayService gameplayService)
        {
            _gameplayService = gameplayService;
        }

        protected override void OnExecute(RhythmInput input)
        {
            _gameplayService.SubmitInput(input);
        }
    }
}
