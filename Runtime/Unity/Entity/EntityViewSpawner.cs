using System;
using System.Collections.Generic;
using System.Linq;
using MyArchitecture.Core;
using UnityEngine;
using VContainer;

namespace MyArchitecture.Unity
{
    public abstract class EntityViewSpawner<TEntityId, TData, TView> :
        View,
        IEntityViewSpawner<TEntityId, TData, TView>
        where TEntityId : notnull
        where TView : Component, IEntityView<TEntityId>
    {
        [SerializeField] private TView prefab;
        [SerializeField] private Transform parent;
        [SerializeField] private EntityViewDespawnMode despawnMode = EntityViewDespawnMode.Destroy;
        [SerializeField] private int poolWarmupCount;
        [SerializeField] private int maxInactiveCount = -1;

        private readonly HashSet<TView> _spawnedViews = new();
        private readonly Dictionary<TView, EntityViewDespawnMode> _despawnModesByView = new();

        private EntityViewRegistry<TEntityId, TView> _registry;
        private EntityViewFactory<TEntityId, TView> _factory;
        private PooledEntityViewFactory<TEntityId, TView> _pool;

        public EntityViewDespawnMode DespawnMode => despawnMode;

        [Inject]
        private void InjectEntityViewSpawnerDependencies(
            EntityViewRegistry<TEntityId, TView> registry,
            EntityViewFactory<TEntityId, TView> factory,
            PooledEntityViewFactory<TEntityId, TView> pool)
        {
            _registry = registry;
            _factory = factory;
            _pool = pool;

            ConfigurePool(pool);
        }

        public virtual bool CanSpawn(TEntityId id, TData data) => ShouldSpawn(id, data);

        public TView Spawn(
            TEntityId id,
            TData data)
        {
            if (!ShouldSpawn(id, data)) return null;

            if (_registry.Contains(id))
            {
                throw new InvalidOperationException(
                    $"View is already registered: {typeof(TEntityId).Name} = {id}");
            }

            TView targetPrefab = GetPrefab(id, data);

            if (targetPrefab == null)
            {
                throw new InvalidOperationException(
                    $"EntityViewSpawner prefab is not set: {GetType().Name}, {typeof(TEntityId).Name} = {id}");
            }

            Transform targetParent = GetParent(id, data);
            Vector3 position = GetPosition(id, data);
            Quaternion rotation = GetRotation(id, data);
            EntityViewDespawnMode mode = despawnMode;

            ThrowIfFactoryIsMissing(mode);

            TView view = null;

            try
            {
                view = mode == EntityViewDespawnMode.Pool
                    ? _pool.Create(
                        targetPrefab,
                        id,
                        position,
                        rotation,
                        targetParent,
                        register: false)
                    : _factory.Create(
                        targetPrefab,
                        id,
                        position,
                        rotation,
                        targetParent,
                        register: false);

                _spawnedViews.Add(view);
                _despawnModesByView[view] = mode;

                _registry.Register(id, view);

                if (Owns(view))
                {
                    OnSpawned(id, data, view);
                }

                return view;
            }
            catch
            {
                if (view != null &&
                    _spawnedViews.Remove(view))
                {
                    _despawnModesByView.Remove(view);
                    CleanupFailedSpawn(view, mode);
                }

                throw;
            }
        }

        public bool Owns(TView view) => view != null && _spawnedViews.Contains(view);

        public void Despawn(
            TEntityId id,
            TView view)
        {
            if (!Owns(view)) return;

            EntityViewDespawnMode mode = _despawnModesByView.GetValueOrDefault(view, despawnMode);

            _spawnedViews.Remove(view);
            _despawnModesByView.Remove(view);

            switch (mode)
            {
                case EntityViewDespawnMode.Destroy:
                    _factory.Destroy(view);
                    break;
                case EntityViewDespawnMode.Pool:
                    _pool.Release(view);
                    break;
                case EntityViewDespawnMode.Keep:
                    _registry.Unregister(view);
                    break;
            }

            OnDespawned(id, view, mode);
        }

        protected virtual bool ShouldSpawn(TEntityId id, TData data) => true;
        protected virtual TView GetPrefab(TEntityId id, TData data) => prefab;
        protected virtual Transform GetParent(TEntityId id, TData data) => parent;
        protected virtual Vector3 GetPosition(TEntityId id, TData data) => transform.position;
        protected virtual Quaternion GetRotation(TEntityId id, TData data) => transform.rotation;

        protected virtual void OnSpawned(TEntityId id, TData data, TView view)
        {
        }

        protected virtual void OnDespawned(
            TEntityId id,
            TView view,
            EntityViewDespawnMode mode)
        {
        }

        protected virtual void ConfigurePool(IEntityViewPoolControl<TView> pool)
        {
            if (despawnMode != EntityViewDespawnMode.Pool ||
                prefab == null ||
                pool == null)
            {
                return;
            }

            if (maxInactiveCount >= 0)
            {
                pool.SetMaxInactiveCount(prefab, maxInactiveCount);
            }

            if (poolWarmupCount > 0)
            {
                pool.Warmup(prefab, poolWarmupCount, parent);
            }
        }

        protected override void OnViewDestroy()
        {
            foreach (var view in _spawnedViews.ToArray())
            {
                if (view == null) continue;

                TEntityId id = view.HasEntity
                    ? view.EntityId
                    : default;

                Despawn(id, view);
            }

            _spawnedViews.Clear();
            _despawnModesByView.Clear();

            base.OnViewDestroy();
        }

        private void CleanupFailedSpawn(
            TView view,
            EntityViewDespawnMode mode)
        {
            if (mode == EntityViewDespawnMode.Pool)
            {
                _pool.Release(view);
                return;
            }

            _factory.Destroy(view);
        }

        private void ThrowIfFactoryIsMissing(EntityViewDespawnMode mode)
        {
            if (mode == EntityViewDespawnMode.Pool)
            {
                if (_pool == null)
                {
                    throw new InvalidOperationException(
                        $"PooledEntityViewFactory is not injected: {GetType().Name}");
                }

                return;
            }

            if (_factory == null)
            {
                throw new InvalidOperationException(
                    $"EntityViewFactory is not injected: {GetType().Name}");
            }
        }
    }
}
