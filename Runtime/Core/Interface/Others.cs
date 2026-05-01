using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace MyArchitecture.Core
{
    public interface IArchitectureObject
    {
    }

    public interface IArchitectureInitializable
    {
        int InitializeOrder { get; }

        void Initialize();
        void Bind();
        void PostInitialize();
    }

    public interface IArchitectureDisposable : IDisposable
    {
    }

    public interface IArchitectureLogger
    {
        void Log(string message);
        void Warning(string message);
        void Error(string message);
        void Exception(Exception exception);
    }

    public interface ISceneLoader
    {
        string ActiveSceneName { get; }
        int ActiveSceneBuildIndex { get; }

        UniTask LoadAsync(
            string sceneName,
            LoadSceneMode mode = LoadSceneMode.Single,
            IProgress<float> progress = null,
            CancellationToken cancellationToken = default);

        UniTask LoadAsync(
            int buildIndex,
            LoadSceneMode mode = LoadSceneMode.Single,
            IProgress<float> progress = null,
            CancellationToken cancellationToken = default);

        UniTask UnloadAsync(
            string sceneName,
            IProgress<float> progress = null,
            CancellationToken cancellationToken = default);

        UniTask UnloadAsync(
            int buildIndex,
            IProgress<float> progress = null,
            CancellationToken cancellationToken = default);

        bool IsLoaded(string sceneName);
        bool IsLoaded(int buildIndex);
        bool SetActiveScene(string sceneName);
        bool SetActiveScene(int buildIndex);
    }
}
