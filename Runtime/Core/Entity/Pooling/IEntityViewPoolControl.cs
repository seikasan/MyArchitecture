using System;
using UnityEngine;

namespace MyArchitecture.Core
{
    public interface IEntityViewPoolControl<TView> :
        IDisposable
        where TView : Component
    {
        int DefaultMaxInactiveCount { get; set; }

        void SetMaxInactiveCount(TView prefab, int maxInactiveCount);

        void Warmup(
            TView prefab,
            int count,
            Transform parent = null);

        void ClearInactive();
    }
}