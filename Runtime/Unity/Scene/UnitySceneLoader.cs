using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MyArchitecture.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyArchitecture.Unity
{
    public sealed class UnitySceneLoader : ISceneLoader
    {
        public string ActiveSceneName => SceneManager.GetActiveScene().name;
        public int ActiveSceneBuildIndex => SceneManager.GetActiveScene().buildIndex;

        public UniTask LoadAsync(
            string sceneName,
            LoadSceneMode mode = LoadSceneMode.Single,
            IProgress<float> progress = null,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(sceneName))
            {
                throw new ArgumentException("Scene name is null or empty.", nameof(sceneName));
            }

            var operation = SceneManager.LoadSceneAsync(sceneName, mode);
            return AwaitOperation(operation, $"Load scene '{sceneName}'", progress, cancellationToken);
        }

        public UniTask LoadAsync(
            int buildIndex,
            LoadSceneMode mode = LoadSceneMode.Single,
            IProgress<float> progress = null,
            CancellationToken cancellationToken = default)
        {
            if (buildIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(buildIndex));
            }

            var operation = SceneManager.LoadSceneAsync(buildIndex, mode);
            return AwaitOperation(operation, $"Load scene buildIndex '{buildIndex}'", progress, cancellationToken);
        }

        public UniTask UnloadAsync(
            string sceneName,
            IProgress<float> progress = null,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(sceneName))
            {
                throw new ArgumentException("Scene name is null or empty.", nameof(sceneName));
            }

            var operation = SceneManager.UnloadSceneAsync(sceneName);
            return AwaitOperation(operation, $"Unload scene '{sceneName}'", progress, cancellationToken);
        }

        public UniTask UnloadAsync(
            int buildIndex,
            IProgress<float> progress = null,
            CancellationToken cancellationToken = default)
        {
            if (buildIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(buildIndex));
            }

            var operation = SceneManager.UnloadSceneAsync(buildIndex);
            return AwaitOperation(operation, $"Unload scene buildIndex '{buildIndex}'", progress, cancellationToken);
        }

        public bool IsLoaded(string sceneName)
        {
            if (string.IsNullOrWhiteSpace(sceneName))
            {
                return false;
            }

            var scene = SceneManager.GetSceneByName(sceneName);
            return scene.IsValid() && scene.isLoaded;
        }

        public bool IsLoaded(int buildIndex)
        {
            if (buildIndex < 0)
            {
                return false;
            }

            var scene = SceneManager.GetSceneByBuildIndex(buildIndex);
            return scene.IsValid() && scene.isLoaded;
        }

        public bool SetActiveScene(string sceneName)
        {
            if (string.IsNullOrWhiteSpace(sceneName))
            {
                return false;
            }

            var scene = SceneManager.GetSceneByName(sceneName);
            return SetActiveScene(scene);
        }

        public bool SetActiveScene(int buildIndex)
        {
            if (buildIndex < 0)
            {
                return false;
            }

            var scene = SceneManager.GetSceneByBuildIndex(buildIndex);
            return SetActiveScene(scene);
        }

        private static bool SetActiveScene(Scene scene)
        {
            return scene.IsValid() &&
                   scene.isLoaded &&
                   SceneManager.SetActiveScene(scene);
        }

        private static UniTask AwaitOperation(
            AsyncOperation operation,
            string operationName,
            IProgress<float> progress,
            CancellationToken cancellationToken)
        {
            if (operation is null)
            {
                throw new InvalidOperationException($"{operationName} failed to start.");
            }

            return operation.ToUniTask(progress, cancellationToken: cancellationToken);
        }
    }
}
