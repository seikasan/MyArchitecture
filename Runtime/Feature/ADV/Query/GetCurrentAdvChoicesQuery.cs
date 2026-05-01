using System.Collections.Generic;
using MyArchitecture.Core;

namespace MyArchitecture.Feature.ADV
{
    public sealed class GetCurrentAdvChoicesQuery : Query<IReadOnlyList<AdvChoice>>
    {
        private readonly IReadOnlyAdvScenarioModel _scenarioModel;

        public GetCurrentAdvChoicesQuery(
            IReadOnlyAdvScenarioModel scenarioModel)
        {
            _scenarioModel = scenarioModel;
        }

        protected override IReadOnlyList<AdvChoice> OnExecute()
        {
            return _scenarioModel.CurrentChoices;
        }
    }
}
