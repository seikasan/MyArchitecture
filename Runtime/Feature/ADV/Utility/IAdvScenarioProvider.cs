using System.Threading;
using Cysharp.Threading.Tasks;
using MyArchitecture.Core;

namespace MyArchitecture.Feature.ADV
{
    public interface IAdvScenarioProvider : IUtility
    {
        bool CanProvide(string scenarioId);

        UniTask<AdvScenario> LoadAsync(
            string scenarioId,
            CancellationToken cancellationToken);
    }
}