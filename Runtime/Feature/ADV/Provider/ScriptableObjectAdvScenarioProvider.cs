using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MyArchitecture.Core;

namespace MyArchitecture.Feature.ADV
{
    public sealed class ScriptableObjectAdvScenarioProvider :
        Utility,
        IAdvScenarioProvider
    {
        private const string Prefix = "adv-so:";

        private readonly IAssetLoader _assetLoader;

        public ScriptableObjectAdvScenarioProvider(IAssetLoader assetLoader)
        {
            _assetLoader = assetLoader;
        }

        public bool CanProvide(string scenarioId)
        {
            return scenarioId != null &&
                   scenarioId.StartsWith(Prefix, StringComparison.Ordinal);
        }

        public async UniTask<AdvScenario> LoadAsync(
            string scenarioId,
            CancellationToken cancellationToken)
        {
            string key = scenarioId.Substring(Prefix.Length);
            AdvScenarioAssetSO asset = await _assetLoader.LoadAsync<AdvScenarioAssetSO>(
                key,
                cancellationToken);

            return asset.ToScenario();
        }
    }
}