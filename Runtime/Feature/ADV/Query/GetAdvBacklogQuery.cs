using System.Collections.Generic;
using MyArchitecture.Core;

namespace MyArchitecture.Feature.ADV
{
    public sealed class GetAdvBacklogQuery : Query<IReadOnlyList<AdvBacklogEntry>>
    {
        private readonly IReadOnlyAdvScenarioModel _scenarioModel;

        public GetAdvBacklogQuery(
            IReadOnlyAdvScenarioModel scenarioModel)
        {
            _scenarioModel = scenarioModel;
        }

        protected override IReadOnlyList<AdvBacklogEntry> OnExecute()
        {
            return _scenarioModel.Backlog;
        }
    }
}
