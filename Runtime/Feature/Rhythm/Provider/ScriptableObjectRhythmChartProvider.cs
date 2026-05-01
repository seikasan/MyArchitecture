using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MyArchitecture.Core;

namespace MyArchitecture.Feature.Rhythm
{
    public sealed class ScriptableObjectRhythmChartProvider :
        Utility,
        IRhythmChartProvider
    {
        private const string Prefix = "rhythm-so:";

        private readonly IAssetLoader _assetLoader;

        public ScriptableObjectRhythmChartProvider(IAssetLoader assetLoader)
        {
            _assetLoader = assetLoader;
        }

        public bool CanProvide(string chartId)
        {
            return chartId != null &&
                   chartId.StartsWith(Prefix, StringComparison.Ordinal);
        }

        public async UniTask<RhythmChart> LoadAsync(
            string chartId,
            CancellationToken cancellationToken)
        {
            string key = chartId.Substring(Prefix.Length);
            RhythmChartAssetSO asset = await _assetLoader.LoadAsync<RhythmChartAssetSO>(
                key,
                cancellationToken);

            return asset.ToChart();
        }
    }
}
