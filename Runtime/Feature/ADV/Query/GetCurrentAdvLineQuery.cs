using MyArchitecture.Core;

namespace MyArchitecture.Feature.ADV
{
    public sealed class GetCurrentAdvLineQuery : Query<AdvLine>
    {
        private readonly IReadOnlyAdvScenarioModel _scenarioModel;

        public GetCurrentAdvLineQuery(
            IReadOnlyAdvScenarioModel scenarioModel)
        {
            _scenarioModel = scenarioModel;
        }

        protected override AdvLine OnExecute()
        {
            return _scenarioModel.CurrentLine;
        }
    }
}
