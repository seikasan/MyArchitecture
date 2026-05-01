using UnityEngine;

namespace MyArchitecture.Core
{
    public enum EntityViewDespawnMode
    {
        Keep,
        Destroy,
        Pool
    }

    public interface IEntityViewSpawner<TEntityId, TData, TView> :
        IView
        where TEntityId : notnull
        where TView : Component, IEntityView<TEntityId>
    {
        bool CanSpawn(TEntityId id, TData data);
        TView Spawn(TEntityId id, TData data);
        bool Owns(TView view);
        void Despawn(TEntityId id, TView view);
    }
}
