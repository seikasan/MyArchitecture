using UnityEngine;

namespace MyArchitecture.Core
{
    public interface IEntityViewPool<TEntityId, TView>
        where TEntityId : notnull
        where TView : Component, IEntityView<TEntityId>
    {
        TView Create(
            TView prefab,
            TEntityId id,
            Vector3 position,
            Quaternion rotation,
            Transform parent = null,
            bool register = true);

        TView Create(
            TView prefab,
            TEntityId id,
            Transform parent,
            bool worldPositionStays = false,
            bool register = true);

        void Release(TView view);
        void Release(TEntityId id);
    }
}
