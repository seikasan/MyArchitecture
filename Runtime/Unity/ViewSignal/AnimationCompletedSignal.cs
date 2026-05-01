using MyArchitecture.Core;
using R3;
using UnityEngine;

namespace MyArchitecture.Unity
{
    [DisallowMultipleComponent]
    public sealed class AnimationCompletedSignal : MonoBehaviour
    {
        private readonly ViewSignal _completed = new();

        public ViewSignal Completed => _completed;

        public Observable<Unit> AsObservable()
            => _completed.AsObservable();

        public void Complete()
            => _completed.Publish();

        private void OnDestroy()
            => _completed.Dispose();
    }
}
