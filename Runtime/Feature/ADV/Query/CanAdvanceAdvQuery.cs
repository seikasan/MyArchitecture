using MyArchitecture.Core;

namespace MyArchitecture.Feature.ADV
{
    public sealed class CanAdvanceAdvQuery : Query<bool>
    {
        private readonly IReadOnlyAdvScenarioModel _scenarioModel;

        public CanAdvanceAdvQuery(
            IReadOnlyAdvScenarioModel scenarioModel)
        {
            _scenarioModel = scenarioModel;
        }

        protected override bool OnExecute()
        {
            return _scenarioModel.PlaybackState ==
                   AdvPlaybackState.WaitingForAdvance;
        }
    }
}
