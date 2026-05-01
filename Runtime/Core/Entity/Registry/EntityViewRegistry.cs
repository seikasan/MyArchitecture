using System;
using System.Collections.Generic;
using R3;

namespace MyArchitecture.Core
{
    public sealed class EntityViewRegistry<TEntityId, TView> :
        IDisposable
        where TEntityId : notnull
        where TView : class, IEntityView<TEntityId>
    {
        private readonly Dictionary<TEntityId, TView> _views = new();
        private readonly Dictionary<TView, TEntityId> _idsByView = new();

        private readonly Subject<EntityViewRegisteredArgs<TEntityId, TView>> _viewRegistered = new();
        private readonly Subject<EntityViewUnregisteredArgs<TEntityId, TView>> _viewUnregistered = new();
        private readonly Subject<Unit> _viewsCleared = new();

        public int Count => _views.Count;

        public IReadOnlyDictionary<TEntityId, TView> Views => _views;

        public Observable<EntityViewRegisteredArgs<TEntityId, TView>> ViewRegistered => _viewRegistered;
        public Observable<EntityViewUnregisteredArgs<TEntityId, TView>> ViewUnregistered => _viewUnregistered;
        public Observable<Unit> ViewsCleared => _viewsCleared;

        public bool Contains(TEntityId id) => _views.ContainsKey(id);
        public bool TryGet(TEntityId id, out TView view) => _views.TryGetValue(id, out view);
        private bool TryFindId(TView view, out TEntityId id) => _idsByView.TryGetValue(view, out id);

        public void Register(TEntityId id, TView view)
        {
            if (view == null)
            {
                throw new ArgumentNullException(nameof(view));
            }

            if (_views.TryGetValue(id, out var currentView))
            {
                if (ReferenceEquals(currentView, view))
                {
                    if (!view.HasEntity ||
                        !EqualityComparer<TEntityId>.Default.Equals(view.EntityId, id))
                    {
                        view.BindEntity(id);
                    }

                    return;
                }

                throw new InvalidOperationException(
                    $"View is already registered: {typeof(TEntityId).Name} = {id}");
            }

            if (TryFindId(view, out var currentId))
            {
                throw new InvalidOperationException(
                    $"View is already registered: {typeof(TEntityId).Name} = {currentId}");
            }

            if (view.HasEntity &&
                !EqualityComparer<TEntityId>.Default.Equals(view.EntityId, id))
            {
                throw new InvalidOperationException(
                    $"View is already bound: {typeof(TEntityId).Name} = {view.EntityId}");
            }

            view.BindEntity(id);

            _views.Add(id, view);
            _idsByView.Add(view, id);

            _viewRegistered.OnNext(
                new EntityViewRegisteredArgs<TEntityId, TView>(id, view));
        }

        public bool Unregister(TEntityId id)
        {
            if (!_views.Remove(id, out TView view)) return false;

            _idsByView.Remove(view);
            view.UnbindEntity();

            _viewUnregistered.OnNext(
                new EntityViewUnregisteredArgs<TEntityId, TView>(id, view));

            return true;
        }

        public bool Unregister(TView view)
        {
            if (view == null) return false;
            if (!_idsByView.Remove(view, out var id)) return false;

            _views.Remove(id);
            view.UnbindEntity();

            _viewUnregistered.OnNext(
                new EntityViewUnregisteredArgs<TEntityId, TView>(
                    id,
                    view));

            return true;
        }

        internal bool UnregisterSilently(TEntityId id)
        {
            if (!_views.Remove(id, out var view)) return false;

            _idsByView.Remove(view);
            view.UnbindEntity();

            return true;
        }

        internal bool UnregisterSilently(TView view)
        {
            if (view == null) return false;

            if (!_idsByView.Remove(view, out var id)) return false;

            _views.Remove(id);
            view.UnbindEntity();

            return true;
        }

        public void Clear()
        {
            if (_views.Count == 0) return;

            var pairs = new List<KeyValuePair<TEntityId, TView>>(_views);

            _views.Clear();
            _idsByView.Clear();

            foreach (var pair in pairs)
            {
                pair.Value.UnbindEntity();

                _viewUnregistered.OnNext(
                    new EntityViewUnregisteredArgs<TEntityId, TView>(
                        pair.Key,
                        pair.Value));
            }

            _viewsCleared.OnNext(Unit.Default);
        }

        internal void ClearSilently()
        {
            if (_views.Count == 0) return;

            var views = new List<TView>(_views.Values);

            _views.Clear();
            _idsByView.Clear();

            foreach (var view in views)
            {
                view.UnbindEntity();
            }
        }

        public void Dispose()
        {
            ClearSilently();

            _viewRegistered.Dispose();
            _viewUnregistered.Dispose();
            _viewsCleared.Dispose();
        }
    }
}
