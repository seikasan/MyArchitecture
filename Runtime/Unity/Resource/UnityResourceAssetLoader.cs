using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MyArchitecture.Core;
using UnityEngine;

namespace MyArchitecture.Unity
{
    public sealed class UnityResourceAssetLoader : IAssetLoader
    {
        public async UniTask<T> LoadAsync<T>(
            string key,
            CancellationToken cancellationToken)
            where T : class
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException(
                    "Asset key is null or empty.",
                    nameof(key));
            }

            cancellationToken.ThrowIfCancellationRequested();

            ResourceRequest request = Resources.LoadAsync(key, typeof(T));

            await request.ToUniTask(cancellationToken: cancellationToken);

            if (request.asset is T asset)
            {
                return asset;
            }

            throw new InvalidOperationException(
                $"Asset load failed. Key: {key}, Type: {typeof(T).FullName}");
        }

        public void Release(object asset)
        {
            if (asset is null)
            {
                return;
            }

            Resources.UnloadAsset(asset as UnityEngine.Object);
        }
    }
}
