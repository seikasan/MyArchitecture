using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MyArchitecture.Core;
using UnityEngine;

namespace MyArchitecture.Feature.ADV
{
    public sealed class JsonAdvScenarioProvider :
        Utility,
        IAdvScenarioProvider
    {
        private const string Prefix = "adv-json:";

        private readonly IAssetLoader _assetLoader;

        public JsonAdvScenarioProvider(IAssetLoader assetLoader)
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
            TextAsset asset = await _assetLoader.LoadAsync<TextAsset>(
                key,
                cancellationToken);

            var scenarioData = JsonUtility.FromJson<AdvScenarioData>(asset.text);

            return scenarioData.ToScenario();
        }
    }
}