using System.Threading;
using Cysharp.Threading.Tasks;

namespace MyArchitecture.Core
{
    public interface ITimeProvider : IUtility
    {
        float Time { get; }
        float DeltaTime { get; }
        float UnscaledTime { get; }
        float UnscaledDeltaTime { get; }
    }

    public interface IAssetLoader : IUtility
    {
        UniTask<T> LoadAsync<T>(
            string key,
            CancellationToken cancellationToken)
            where T : class;

        void Release(object asset);
    }

    public interface ISaveDataRepository<TSaveData> : IUtility
    {
        UniTask SaveAsync(
            TSaveData saveData,
            CancellationToken cancellationToken);

        UniTask<TSaveData> LoadAsync(
            CancellationToken cancellationToken);

        UniTask<bool> ExistsAsync(
            CancellationToken cancellationToken);
    }
}