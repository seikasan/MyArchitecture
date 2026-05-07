using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;
using VContainer;

namespace MyArchitecture.Core
{
    public sealed class PooledEntityViewFactory<TEntityId, TView> :
        EntityViewFactoryBase<TEntityId, TView>,
        IEntityViewPool<TEntityId, TView>,
        IEntityViewPoolControl<TView>
        where TEntityId : notnull
        where TView : Component, IEntityView<TEntityId>
    {
        private readonly struct PoolKey : IEquatable<PoolKey>
        {
            public PoolKey(TView prefab)
            {
                PrefabInstanceId = prefab.GetInstanceID();
            }

            public int PrefabInstanceId { get; }

            public bool Equals(PoolKey other) => PrefabInstanceId == other.PrefabInstanceId;

            public override bool Equals(object obj) => obj is PoolKey other && Equals(other);

            public override int GetHashCode() => PrefabInstanceId;
        }

        private enum PoolCreateMode
        {
            PositionAndRotation,
            Parent
        }

        private readonly struct PoolCreateContext
        {
            private PoolCreateContext(
                PoolKey poolKey,
                PoolCreateMode mode,
                Vector3 position,
                Quaternion rotation,
                Transform parent,
                bool worldPositionStays)
            {
                PoolKey = poolKey;
                Mode = mode;
                Position = position;
                Rotation = rotation;
                Parent = parent;
                WorldPositionStays = worldPositionStays;
            }

            public PoolKey PoolKey { get; }
            public PoolCreateMode Mode { get; }
            public Vector3 Position { get; }
            public Quaternion Rotation { get; }
            public Transform Parent { get; }
            public bool WorldPositionStays { get; }

            public static PoolCreateContext ForPositionAndRotation(
                PoolKey poolKey,
                Vector3 position,
                Quaternion rotation,
                Transform parent)
                => new(
                    poolKey,
                    PoolCreateMode.PositionAndRotation,
                    position,
                    rotation,
                    parent,
                    false);

            public static PoolCreateContext ForParent(
                PoolKey poolKey,
                Transform parent,
                bool worldPositionStays)
                => new(
                    poolKey,
                    PoolCreateMode.Parent,
                    default,
                    default,
                    parent,
                    worldPositionStays);
        }

        private const int PoolDefaultCapacity = 10;

        private readonly Dictionary<PoolKey, ObjectPool<TView>> _poolsByKey = new();
        private readonly Dictionary<PoolKey, HashSet<TView>> _inactiveViewSetsByKey = new();
        private readonly Dictionary<TView, PoolKey> _poolKeyByCreatedView = new();
        private readonly HashSet<TView> _createdViews = new();

        private int _defaultMaxInactiveCount = 128;
        private readonly Dictionary<PoolKey, int> _maxInactiveCountByKey = new();
        private bool _hasCreateContext;
        private bool _lastGetCreated;
        private PoolCreateContext _createContext;

        public int DefaultMaxInactiveCount
        {
            get => _defaultMaxInactiveCount;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                _defaultMaxInactiveCount = value;
            }
        }

        public PooledEntityViewFactory(
            IObjectResolver resolver,
            EntityViewRegistry<TEntityId, TView> registry) : base(resolver, registry)
        {
        }

        public void Dispose()
        {
            foreach (var pool in _poolsByKey.Values)
            {
                pool.Dispose();
            }

            var views = _createdViews.ToArray();

            foreach (var view in views)
            {
                DestroyCreatedViewSilently(view);
            }

            _poolsByKey.Clear();
            _inactiveViewSetsByKey.Clear();
            _poolKeyByCreatedView.Clear();
            _createdViews.Clear();
            _maxInactiveCountByKey.Clear();
        }

        public override TView Create(
            TView prefab,
            TEntityId id,
            Vector3 position,
            Quaternion rotation,
            Transform parent = null,
            bool register = true)
        {
            ThrowIfPrefabIsNull(prefab);
            ThrowIfAlreadyRegistered(id, register);

            var poolKey = new PoolKey(prefab);

            var view = GetFromPool(
                prefab,
                poolKey,
                PoolCreateContext.ForPositionAndRotation(
                    poolKey,
                    position,
                    rotation,
                    parent),
                out bool created);

            try
            {
                ApplyTransform(view, prefab, position, rotation, parent);
            }
            catch
            {
                CleanupFailedCreate(view, poolKey, created);
                throw;
            }

            return Activate(view, poolKey, id, register, created);
        }

        public override TView Create(
            TView prefab,
            TEntityId id,
            Transform parent,
            bool worldPositionStays = false,
            bool register = true)
        {
            ThrowIfPrefabIsNull(prefab);
            ThrowIfAlreadyRegistered(id, register);

            var poolKey = new PoolKey(prefab);

            var view = GetFromPool(
                prefab,
                poolKey,
                PoolCreateContext.ForParent(
                    poolKey,
                    parent,
                    worldPositionStays),
                out bool created);

            try
            {
                ApplyTransform(view, prefab, parent, worldPositionStays);
            }
            catch
            {
                CleanupFailedCreate(view, poolKey, created);
                throw;
            }

            return Activate(view, poolKey, id, register, created);
        }

        public void Release(TView view)
        {
            if (view == null) return;

            if (!_createdViews.Contains(view))
            {
                throw new InvalidOperationException(
                    $"View was not created by this pooled factory: {typeof(TView).Name}");
            }

            if (!_poolKeyByCreatedView.TryGetValue(view, out var poolKey))
            {
                throw new InvalidOperationException(
                    $"Pool key is not found for view: {typeof(TView).Name}");
            }

            var inactiveViewSet = GetInactiveViewSet(poolKey);
            if (inactiveViewSet.Contains(view)) return;

            if (!Registry.Unregister(view))
            {
                view.UnbindEntity();
            }

            var maxInactiveCount = GetMaxInactiveCount(poolKey);
            if (inactiveViewSet.Count >= maxInactiveCount)
            {
                DestroyCreatedView(view);
                return;
            }

            NotifyReturnedToPool(view);

            view.gameObject.SetActive(false);

            GetPool(poolKey).Release(view);
        }

        public void Release(TEntityId id)
        {
            if (!Registry.TryGet(id, out var view)) return;

            Release(view);
        }

        public void ClearInactive()
        {
            foreach (var pool in _poolsByKey.Values)
            {
                pool.Clear();
            }

            _inactiveViewSetsByKey.Clear();
        }

        public void SetMaxInactiveCount(TView prefab, int maxInactiveCount)
        {
            ThrowIfPrefabIsNull(prefab);

            if (maxInactiveCount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxInactiveCount));
            }

            _maxInactiveCountByKey[new PoolKey(prefab)] = maxInactiveCount;
        }

        public void Warmup(
            TView prefab,
            int count,
            Transform parent = null)
        {
            ThrowIfPrefabIsNull(prefab);

            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            var poolKey = new PoolKey(prefab);
            var inactiveViewSet = GetInactiveViewSet(poolKey);
            int targetInactiveCount = Math.Min(count, GetMaxInactiveCount(poolKey));

            if (inactiveViewSet.Count >= targetInactiveCount)
            {
                return;
            }

            var pool = GetPool(poolKey, prefab);
            var views = new List<TView>(targetInactiveCount);

            try
            {
                while (views.Count < targetInactiveCount)
                {
                    var view = GetFromPool(
                        prefab,
                        poolKey,
                        PoolCreateContext.ForParent(
                            poolKey,
                            parent,
                            false),
                        out _);

                    view.UnbindEntity();
                    view.gameObject.SetActive(false);

                    views.Add(view);
                }
            }
            finally
            {
                foreach (var view in views)
                {
                    if (view == null) continue;

                    pool.Release(view);
                }
            }
        }

        private int GetMaxInactiveCount(PoolKey poolKey)
            => _maxInactiveCountByKey.GetValueOrDefault(poolKey, _defaultMaxInactiveCount);

        private TView Activate(
            TView view,
            PoolKey poolKey,
            TEntityId id,
            bool register,
            bool created)
        {
            try
            {
                BindOrRegister(view, id, register);

                NotifyRentedFromPool(view, id);

                view.gameObject.SetActive(true);

                return view;
            }
            catch
            {
                CleanupFailedCreate(view, poolKey, created);
                throw;
            }
        }

        private TView GetFromPool(
            TView prefab,
            PoolKey poolKey,
            PoolCreateContext createContext,
            out bool created)
        {
            var pool = GetPool(poolKey, prefab);

            while (true)
            {
                _hasCreateContext = true;
                _createContext = createContext;
                _lastGetCreated = false;

                try
                {
                    var view = pool.Get();
                    created = _lastGetCreated;

                    if (view != null)
                    {
                        return view;
                    }

                    RemoveCreatedViewMetadata(view);
                }
                finally
                {
                    _hasCreateContext = false;
                    _lastGetCreated = false;
                }
            }
        }

        private ObjectPool<TView> GetPool(PoolKey poolKey)
        {
            if (_poolsByKey.TryGetValue(poolKey, out var pool)) return pool;

            throw new InvalidOperationException(
                $"Pool is not found for view: {typeof(TView).Name}");
        }

        private ObjectPool<TView> GetPool(
            PoolKey poolKey,
            TView prefab)
        {
            if (_poolsByKey.TryGetValue(poolKey, out var pool)) return pool;

            pool = new ObjectPool<TView>(
                () => CreatePooledView(prefab, poolKey),
                view => RemoveFromInactiveSet(poolKey, view),
                view => AddToInactiveSet(poolKey, view),
                DestroyCreatedView,
                true,
                PoolDefaultCapacity,
                int.MaxValue);

            _poolsByKey.Add(poolKey, pool);
            return pool;
        }

        private HashSet<TView> GetInactiveViewSet(PoolKey poolKey)
        {
            if (_inactiveViewSetsByKey.TryGetValue(poolKey, out var set)) return set;

            set = new HashSet<TView>();
            _inactiveViewSetsByKey.Add(poolKey, set);
            return set;
        }

        private TView CreatePooledView(
            TView prefab,
            PoolKey poolKey)
        {
            TView view = null;

            try
            {
                view = InstantiatePooledView(prefab, poolKey);

                Resolver.Inject(view);

                _createdViews.Add(view);
                _poolKeyByCreatedView.Add(view, poolKey);
                _lastGetCreated = true;

                return view;
            }
            catch
            {
                if (view != null)
                {
                    UnityEngine.Object.Destroy(view.gameObject);
                }

                throw;
            }
        }

        private TView InstantiatePooledView(
            TView prefab,
            PoolKey poolKey)
        {
            if (!_hasCreateContext ||
                !_createContext.PoolKey.Equals(poolKey))
            {
                return UnityEngine.Object.Instantiate(prefab);
            }

            return _createContext.Mode switch
            {
                PoolCreateMode.PositionAndRotation => UnityEngine.Object.Instantiate(
                    prefab,
                    _createContext.Position,
                    _createContext.Rotation,
                    _createContext.Parent),
                PoolCreateMode.Parent => UnityEngine.Object.Instantiate(
                    prefab,
                    _createContext.Parent,
                    _createContext.WorldPositionStays),
                _ => UnityEngine.Object.Instantiate(prefab)
            };
        }

        private void AddToInactiveSet(
            PoolKey poolKey,
            TView view)
        {
            if (ReferenceEquals(view, null)) return;

            GetInactiveViewSet(poolKey).Add(view);
        }

        private void RemoveFromInactiveSet(
            PoolKey poolKey,
            TView view)
        {
            if (ReferenceEquals(view, null)) return;

            if (_inactiveViewSetsByKey.TryGetValue(poolKey, out var set))
            {
                set.Remove(view);
            }
        }

        private void CleanupFailedCreate(
            TView view,
            PoolKey poolKey,
            bool created)
        {
            if (view == null) return;

            if (!Registry.Unregister(view))
            {
                view.UnbindEntity();
            }

            if (created)
            {
                DestroyCreatedView(view);
                return;
            }

            view.gameObject.SetActive(false);

            GetPool(poolKey).Release(view);
        }

        private void ThrowIfPrefabIsNull(TView prefab)
        {
            if (prefab == null)
            {
                throw new ArgumentNullException(nameof(prefab));
            }
        }

        private void ApplyTransform(
            TView view,
            TView prefab,
            Vector3 position,
            Quaternion rotation,
            Transform parent)
        {
            Transform transform = view.transform;

            transform.SetParent(parent, false);
            transform.SetPositionAndRotation(position, rotation);
            transform.localScale = prefab.transform.localScale;
        }

        private void ApplyTransform(
            TView view,
            TView prefab,
            Transform parent,
            bool worldPositionStays)
        {
            Transform transform = view.transform;

            transform.SetParent(parent, worldPositionStays);

            if (worldPositionStays)
            {
                transform.SetPositionAndRotation(
                    prefab.transform.position,
                    prefab.transform.rotation);
            }
            else
            {
                transform.localPosition = prefab.transform.localPosition;
                transform.localRotation = prefab.transform.localRotation;
            }

            transform.localScale = prefab.transform.localScale;
        }

        private void DestroyCreatedView(TView view)
        {
            if (ReferenceEquals(view, null)) return;

            if (view != null &&
                !Registry.Unregister(view))
            {
                view.UnbindEntity();
            }

            RemoveCreatedViewMetadata(view);

            if (view != null)
            {
                UnityEngine.Object.Destroy(view.gameObject);
            }
        }

        private static void NotifyRentedFromPool(TView view, TEntityId id)
        {
            if (view is IPooledEntityView<TEntityId> pooled)
            {
                pooled.OnRentFromPool(id);
            }
        }

        private static void NotifyReturnedToPool(TView view)
        {
            if (view is IPooledEntityView<TEntityId> pooled)
            {
                pooled.OnReturnToPool();
            }
        }

        private void DestroyCreatedViewSilently(TView view)
        {
            if (ReferenceEquals(view, null)) return;

            if (view != null &&
                !Registry.UnregisterSilently(view))
            {
                view.UnbindEntity();
            }

            RemoveCreatedViewMetadata(view);

            if (view != null)
            {
                UnityEngine.Object.Destroy(view.gameObject);
            }
        }

        private void RemoveCreatedViewMetadata(TView view)
        {
            if (ReferenceEquals(view, null)) return;

            _createdViews.Remove(view);

            if (_poolKeyByCreatedView.Remove(view, out var poolKey) &&
                _inactiveViewSetsByKey.TryGetValue(poolKey, out var inactiveSet))
            {
                inactiveSet.Remove(view);
            }
        }
    }
}
