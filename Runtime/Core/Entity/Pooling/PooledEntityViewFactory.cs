using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
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

        private readonly Dictionary<PoolKey, Stack<TView>> _inactiveViewsByKey = new();
        private readonly Dictionary<PoolKey, HashSet<TView>> _inactiveViewSetsByKey = new();
        private readonly Dictionary<TView, PoolKey> _poolKeyByCreatedView = new();
        private readonly HashSet<TView> _createdViews = new();

        private int _defaultMaxInactiveCount = 128;
        private readonly Dictionary<PoolKey, int> _maxInactiveCountByKey = new();

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
            var views = _createdViews.ToArray();

            foreach (var view in views)
            {
                DestroyCreatedViewSilently(view);
            }

            _inactiveViewsByKey.Clear();
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

            var view = TryTakeInactive(poolKey);
            bool created = view == null;

            if (created)
            {
                view = UnityEngine.Object.Instantiate(
                    prefab,
                    position,
                    rotation,
                    parent);
            }
            else
            {
                ApplyTransform(view, prefab, position, rotation, parent);
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

            var view = TryTakeInactive(poolKey);
            bool created = view == null;

            if (created)
            {
                view = UnityEngine.Object.Instantiate(
                    prefab,
                    parent,
                    worldPositionStays);
            }
            else
            {
                ApplyTransform(view, prefab, parent, worldPositionStays);
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

            GetInactiveViews(poolKey).Push(view);
            inactiveViewSet.Add(view);
        }

        public void Release(TEntityId id)
        {
            if (!Registry.TryGet(id, out var view)) return;

            Release(view);
        }

        public void ClearInactive()
        {
            foreach (var pair in _inactiveViewsByKey)
            {
                Stack<TView> inactiveViews = pair.Value;

                while (inactiveViews.Count > 0)
                {
                    TView view = inactiveViews.Pop();
                    DestroyCreatedView(view);
                }
            }

            _inactiveViewsByKey.Clear();
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
            var inactiveViews = GetInactiveViews(poolKey);
            var inactiveViewSet = GetInactiveViewSet(poolKey);
            var maxInactiveCount = GetMaxInactiveCount(poolKey);

            while (inactiveViewSet.Count < maxInactiveCount &&
                   inactiveViewSet.Count < count)
            {
                var view = UnityEngine.Object.Instantiate(prefab, parent);
                Resolver.Inject(view);

                view.UnbindEntity();
                view.gameObject.SetActive(false);

                _createdViews.Add(view);
                _poolKeyByCreatedView.Add(view, poolKey);

                inactiveViews.Push(view);
                inactiveViewSet.Add(view);
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
                if (created)
                {
                    _createdViews.Add(view);
                    _poolKeyByCreatedView.Add(view, poolKey);
                    Resolver.Inject(view);
                }

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

        private TView TryTakeInactive(PoolKey poolKey)
        {
            if (!_inactiveViewsByKey.TryGetValue(poolKey, out var inactiveViews)) return null;

            var inactiveViewSet = GetInactiveViewSet(poolKey);

            while (inactiveViews.Count > 0)
            {
                var view = inactiveViews.Pop();
                inactiveViewSet.Remove(view);

                if (view != null) return view;

                _createdViews.Remove(view);
                _poolKeyByCreatedView.Remove(view);
            }

            return null;
        }

        private Stack<TView> GetInactiveViews(PoolKey poolKey)
        {
            if (_inactiveViewsByKey.TryGetValue(poolKey, out var stack)) return stack;

            stack = new Stack<TView>();
            _inactiveViewsByKey.Add(poolKey, stack);
            return stack;
        }

        private HashSet<TView> GetInactiveViewSet(PoolKey poolKey)
        {
            if (_inactiveViewSetsByKey.TryGetValue(poolKey, out var set)) return set;

            set = new HashSet<TView>();
            _inactiveViewSetsByKey.Add(poolKey, set);
            return set;
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
                _createdViews.Remove(view);
                _poolKeyByCreatedView.Remove(view);
                UnityEngine.Object.Destroy(view.gameObject);
                return;
            }

            view.gameObject.SetActive(false);

            var inactiveViewSet = GetInactiveViewSet(poolKey);

            if (inactiveViewSet.Add(view))
            {
                GetInactiveViews(poolKey).Push(view);
            }
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
            if (view == null) return;

            if (!Registry.Unregister(view))
            {
                view.UnbindEntity();
            }

            _createdViews.Remove(view);

            if (_poolKeyByCreatedView.Remove(view, out var poolKey))
            {
                if (_inactiveViewSetsByKey.TryGetValue(poolKey, out var inactiveSet))
                {
                    inactiveSet.Remove(view);
                }
            }

            UnityEngine.Object.Destroy(view.gameObject);
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
            if (view == null) return;

            if (!Registry.UnregisterSilently(view))
            {
                view.UnbindEntity();
            }

            UnityEngine.Object.Destroy(view.gameObject);
        }
    }
}
