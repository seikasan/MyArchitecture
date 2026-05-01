using System;
using System.Collections.Generic;
using System.Linq;
using R3;
using UnityEngine;
using VContainer;

namespace MyArchitecture.Core
{
    public abstract class EntityPresenter<TEntityId, TData, TView> :
        Presenter
        where TEntityId : notnull
        where TView : Component, IEntityView<TEntityId>
    {
        private IReadOnlyEntityCollectionModel<TEntityId, TData> _model;
        private EntityViewRegistry<TEntityId, TView> _views;
        private IReadOnlyList<IEntityViewSpawner<TEntityId, TData, TView>> _spawners =
            Array.Empty<IEntityViewSpawner<TEntityId, TData, TView>>();

        protected IReadOnlyEntityCollectionModel<TEntityId, TData> Model => _model;
        protected EntityViewRegistry<TEntityId, TView> Views => _views;

        [Inject]
        private void InjectEntityPresenterDependencies(
            IReadOnlyEntityCollectionModel<TEntityId, TData> model,
            EntityViewRegistry<TEntityId, TView> views,
            IEnumerable<IEntityViewSpawner<TEntityId, TData, TView>> spawners)
        {
            _model = model;
            _views = views;
            _spawners = spawners?.ToArray() ??
                        Array.Empty<IEntityViewSpawner<TEntityId, TData, TView>>();

            if (_spawners.Count > 1)
            {
                throw new InvalidOperationException(
                    $"Multiple EntityViewSpawner instances are registered for {typeof(TView).Name}.");
            }
        }

        protected override void OnBind()
        {
            Track(_model.Changed.Subscribe(OnChangedInternal));

            Track(_views.ViewRegistered.Subscribe(OnViewRegisteredInternal));

            RefreshExistingPairs();
        }

        public void RefreshAllViews()
        {
            foreach (var pair in _views.Views)
            {
                TEntityId id = pair.Key;
                TView view = pair.Value;

                if (!_model.TryGet(id, out TData data)) continue;

                RefreshView(id, data, view);
            }
        }

        public void RefreshView(TEntityId id)
        {
            if (!_model.TryGet(id, out TData data)) return;
            if (!_views.TryGet(id, out TView view)) return;

            RefreshView(id, data, view);
        }

        private void RefreshExistingPairs()
        {
            foreach (var pair in _model.Snapshot())
            {
                EnsureViewAndRefresh(pair.Key, pair.Value);
            }
        }

        private void OnChangedInternal(EntityChangeSet<TEntityId, TData> changeSet)
        {
            foreach (var change in changeSet)
            {
                switch (change.Operation)
                {
                    case EntityChangeOperation.Added:
                    case EntityChangeOperation.Updated:
                        EnsureViewAndRefresh(
                            change.Id,
                            change.CurrentData);
                        break;

                    case EntityChangeOperation.Removed:
                        OnEntityRemovedInternal(change.Id);
                        break;
                }
            }
        }

        private void OnEntityRemovedInternal(TEntityId id)
        {
            if (!_views.TryGet(id, out TView view)) return;

            DespawnOrUnregister(id, view);
        }

        private void OnViewRegisteredInternal(EntityViewRegisteredArgs<TEntityId, TView> args)
        {
            if (_model.TryGet(args.Id, out TData data))
            {
                RefreshView(args.Id, data, args.View);
            }
        }

        private void EnsureViewAndRefresh(TEntityId id, TData data)
        {
            if (_views.TryGet(id, out TView view))
            {
                RefreshView(id, data, view);
                return;
            }

            if (TrySpawnView(id, data) &&
                _model.Contains(id) &&
                !_views.Contains(id))
            {
                throw new InvalidOperationException(
                    $"EntityViewSpawner spawned a view, but it was not registered: {typeof(TEntityId).Name} = {id}");
            }
        }

        private bool TrySpawnView(TEntityId id, TData data)
        {
            if (_spawners.Count == 0) return false;

            IEntityViewSpawner<TEntityId, TData, TView> spawner = _spawners[0];

            if (!spawner.CanSpawn(id, data)) return false;

            return spawner.Spawn(id, data) != null;
        }

        private void DespawnOrUnregister(TEntityId id, TView view)
        {
            foreach (var spawner in _spawners)
            {
                if (!spawner.Owns(view)) continue;

                spawner.Despawn(id, view);
                return;
            }

            _views.Unregister(id);
        }

        protected virtual void RefreshView(TEntityId id, TData data, TView view)
        {
            if (view is IEntityDataView<TData> dataView)
            {
                dataView.Apply(data);
                return;
            }

            throw new InvalidOperationException(
                $"{typeof(TView).Name} must implement IEntityDataView<{typeof(TData).Name}> " +
                $"or {GetType().Name} must override RefreshView().");
        }
    }
}
