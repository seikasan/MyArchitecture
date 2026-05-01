using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MyArchitecture.Core;
using UnityEngine;

namespace MyArchitecture.Feature.Rhythm
{
    public sealed class JsonRhythmChartProvider :
        Utility,
        IRhythmChartProvider
    {
        private const string Prefix = "rhythm-json:";

        private readonly IAssetLoader _assetLoader;

        public JsonRhythmChartProvider(IAssetLoader assetLoader)
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
            TextAsset asset = await _assetLoader.LoadAsync<TextAsset>(
                key,
                cancellationToken);
            var chartData = JsonUtility.FromJson<RhythmChartData>(asset.text);

            return chartData.ToChart();
        }
    }
}
