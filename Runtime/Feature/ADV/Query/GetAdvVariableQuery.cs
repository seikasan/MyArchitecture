using System.Collections.Generic;
using MyArchitecture.Core;

namespace MyArchitecture.Feature.ADV
{
    public sealed class GetAdvVariableQuery : Query<string, AdvVariableValue>
    {
        private readonly IReadOnlyAdvStateModel _stateModel;

        public GetAdvVariableQuery(
            IReadOnlyAdvStateModel stateModel)
        {
            _stateModel = stateModel;
        }

        protected override AdvVariableValue OnExecute(string key)
        {
            if (_stateModel.Variables.TryGetValue(key, out var value))
            {
                return value;
            }

            throw new KeyNotFoundException(
                $"ADV variable is not found: {key}");
        }
    }
}
