using System;
using System.Collections.Generic;
using System.Linq;
using R3;

namespace MyArchitecture.Core
{
    public abstract class EntityCollectionModel<TEntityId, TData> :
        Model,
        IReadOnlyEntityCollectionModel<TEntityId, TData>
        where TEntityId : notnull
    {
        private readonly Dictionary<TEntityId, TData> _items = new();
        private readonly Dictionary<TEntityId, Subject<EntityChange<TEntityId, TData>>> _changedById = new();
        private readonly Subject<EntityChangeSet<TEntityId, TData>> _changed = new();
        private readonly Subject<int> _countChanged = new();

        public int Count => _items.Count;
        public Observable<int> CountChanged => _countChanged;

        public IEnumerable<TEntityId> Ids => _items.Keys;
        public IEnumerable<KeyValuePair<TEntityId, TData>> Items => _items;

        public Observable<EntityChangeSet<TEntityId, TData>> Changed => _changed;
        public Observable<EntityChange<TEntityId, TData>> ObserveEntity(TEntityId targetId)
        {
            if (!_changedById.TryGetValue(targetId, out var subject))
            {
                subject = new Subject<EntityChange<TEntityId, TData>>();
                _changedById.Add(targetId, subject);
            }

            return subject;
        }

        public bool Contains(TEntityId id) => _items.ContainsKey(id);
        public bool Exists(TEntityId id) => _items.ContainsKey(id);
        public IReadOnlyList<KeyValuePair<TEntityId, TData>> Snapshot() => _items.ToArray();

        public TData Get(TEntityId id)
        {
            if (_items.TryGetValue(id, out var data))
            {
                return data;
            }

            throw new KeyNotFoundException(
                $"Entity is not found: {typeof(TEntityId).Name} = {id}");
        }

        public bool TryGet(TEntityId id, out TData data)
        {
            return _items.TryGetValue(id, out data);
        }

        public bool TryFindFirst(
            Func<TEntityId, TData, bool> predicate,
            out KeyValuePair<TEntityId, TData> result)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            foreach (var pair in _items)
            {
                if (predicate(pair.Key, pair.Value))
                {
                    result = pair;
                    return true;
                }
            }

            result = default;
            return false;
        }

        public List<KeyValuePair<TEntityId, TData>> FindAll(
            Func<TEntityId, TData, bool> predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            var results = new List<KeyValuePair<TEntityId, TData>>();

            foreach (var pair in _items)
            {
                if (predicate(pair.Key, pair.Value))
                {
                    results.Add(pair);
                }
            }

            return results;
        }

        public void Add(TEntityId id, TData data)
        {
            _items.Add(id, data);

            PublishChanged(
                EntityChangeSource.Single,
                EntityChange<TEntityId, TData>.Add(id, data));
        }

        public bool TryAdd(TEntityId id, TData data)
        {
            if (!_items.TryAdd(id, data)) return false;

            PublishChanged(
                EntityChangeSource.Single,
                EntityChange<TEntityId, TData>.Add(id, data));

            return true;
        }

        public void Set(TEntityId id, TData data)
        {
            if (!_items.TryGetValue(id, out var previousData))
            {
                throw new KeyNotFoundException(
                    $"Entity is not found: {typeof(TEntityId).Name} = {id}");
            }

            _items[id] = data;

            PublishChanged(
                EntityChangeSource.Single,
                EntityChange<TEntityId, TData>.Update(id, previousData, data));
        }

        public bool TrySet(TEntityId id, TData data)
        {
            if (!_items.TryGetValue(id, out var previousData)) return false;

            _items[id] = data;

            PublishChanged(
                EntityChangeSource.Single,
                EntityChange<TEntityId, TData>.Update(id, previousData, data));

            return true;
        }

        public EntityChangeOperation Upsert(TEntityId id, TData data)
        {
            if (_items.TryGetValue(id, out var previousData))
            {
                _items[id] = data;

                PublishChanged(
                    EntityChangeSource.Single,
                    EntityChange<TEntityId, TData>.Update(
                        id,
                        previousData,
                        data));

                return EntityChangeOperation.Updated;
            }

            _items.Add(id, data);

            PublishChanged(
                EntityChangeSource.Single,
                EntityChange<TEntityId, TData>.Add(
                    id,
                    data));

            return EntityChangeOperation.Added;
        }

        public void Remove(TEntityId id)
        {
            if (!_items.Remove(id, out var previousData))
            {
                throw new KeyNotFoundException(
                    $"Entity is not found: {typeof(TEntityId).Name} = {id}");
            }

            PublishChanged(
                EntityChangeSource.Single,
                EntityChange<TEntityId, TData>.Remove(id, previousData));
        }

        public bool TryRemove(TEntityId id)
        {
            if (!_items.Remove(id, out var previousData)) return false;

            PublishChanged(
                EntityChangeSource.Single,
                EntityChange<TEntityId, TData>.Remove(id, previousData));

            return true;
        }

        public void Clear()
        {
            if (_items.Count == 0)
            {
                return;
            }

            var changes = new List<EntityChange<TEntityId, TData>>(_items.Count);

            foreach (var pair in _items)
            {
                changes.Add(
                    EntityChange<TEntityId, TData>.Remove(
                        pair.Key,
                        pair.Value));
            }

            _items.Clear();

            PublishChanged(
                EntityChangeSource.Clear,
                changes);
        }

        public void SetMany(IEnumerable<KeyValuePair<TEntityId, TData>> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            ApplyBatch(
                new EntityBatch<TEntityId, TData>(
                    updates: items));
        }

        public void UpsertMany(IEnumerable<KeyValuePair<TEntityId, TData>> items)
        {
            var sourceItems = MaterializeDistinctItems(items);
            var changes = new List<EntityChange<TEntityId, TData>>();
            var comparer = EqualityComparer<TData>.Default;

            foreach (var item in sourceItems)
            {
                if (_items.TryGetValue(item.Key, out var previousData))
                {
                    if (comparer.Equals(previousData, item.Value)) continue;

                    _items[item.Key] = item.Value;

                    changes.Add(
                        EntityChange<TEntityId, TData>.Update(
                            item.Key,
                            previousData,
                            item.Value));

                    continue;
                }

                _items.Add(item.Key, item.Value);

                changes.Add(
                    EntityChange<TEntityId, TData>.Add(
                        item.Key,
                        item.Value));
            }

            PublishChanged(
                EntityChangeSource.Batch,
                changes);
        }

        public void ReplaceAll(IEnumerable<KeyValuePair<TEntityId, TData>> items)
        {
            var sourceItems = MaterializeDistinctItems(items);
            var sourceIds = new HashSet<TEntityId>(
                sourceItems.Select(item => item.Key));
            var changes = new List<EntityChange<TEntityId, TData>>();
            var comparer = EqualityComparer<TData>.Default;

            foreach (var id in _items.Keys.ToArray())
            {
                if (sourceIds.Contains(id)) continue;

                var previousData = _items[id];
                _items.Remove(id);

                changes.Add(
                    EntityChange<TEntityId, TData>.Remove(
                        id,
                        previousData));
            }

            foreach (var item in sourceItems)
            {
                if (_items.TryGetValue(item.Key, out var previousData))
                {
                    if (comparer.Equals(previousData, item.Value)) continue;

                    _items[item.Key] = item.Value;

                    changes.Add(
                        EntityChange<TEntityId, TData>.Update(
                            item.Key,
                            previousData,
                            item.Value));

                    continue;
                }

                _items.Add(item.Key, item.Value);

                changes.Add(
                    EntityChange<TEntityId, TData>.Add(
                        item.Key,
                        item.Value));
            }

            PublishChanged(
                EntityChangeSource.Batch,
                changes);
        }

        public int RemoveMany(IEnumerable<TEntityId> ids)
        {
            if (ids == null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            var changes = new List<EntityChange<TEntityId, TData>>();
            var removedIds = new HashSet<TEntityId>();

            foreach (var id in ids)
            {
                if (!removedIds.Add(id)) continue;
                if (!_items.Remove(id, out var previousData)) continue;

                changes.Add(
                    EntityChange<TEntityId, TData>.Remove(
                        id,
                        previousData));
            }

            PublishChanged(
                EntityChangeSource.Batch,
                changes);

            return changes.Count;
        }

        public int RemoveWhere(Func<TEntityId, TData, bool> predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            if (_items.Count == 0) return 0;

            var targets = new List<TEntityId>();

            foreach (var pair in _items)
            {
                if (predicate(pair.Key, pair.Value))
                {
                    targets.Add(pair.Key);
                }
            }

            if (targets.Count == 0) return 0;

            var changes = new List<EntityChange<TEntityId, TData>>(targets.Count);

            foreach (var id in targets)
            {
                if (!_items.Remove(id, out var previousData)) continue;

                changes.Add(
                    EntityChange<TEntityId, TData>.Remove(
                        id,
                        previousData));
            }

            PublishChanged(
                EntityChangeSource.Batch,
                changes);

            return changes.Count;
        }

        /// セーブ、所持品、ステージ状態、ADV 状態、キャラクター状態向け
        public void ApplyBatch(EntityBatch<TEntityId, TData> batch)
        {
            var adds = EmptyIfNull(batch.Adds).ToArray();
            var updates = EmptyIfNull(batch.Updates).ToArray();
            var removes = EmptyIfNull(batch.Removes).ToArray();

            ValidateBatch(
                new EntityBatch<TEntityId, TData>(
                    adds,
                    updates,
                    removes));

            var changes = new List<EntityChange<TEntityId, TData>>();

            foreach (var item in adds)
            {
                _items.Add(item.Key, item.Value);

                changes.Add(
                    EntityChange<TEntityId, TData>.Add(
                        item.Key,
                        item.Value));
            }

            foreach (var item in updates)
            {
                var previousData = _items[item.Key];

                _items[item.Key] = item.Value;

                changes.Add(
                    EntityChange<TEntityId, TData>.Update(
                        item.Key,
                        previousData,
                        item.Value));
            }

            foreach (var id in removes)
            {
                if (!_items.Remove(id, out var previousData))
                {
                    continue;
                }

                changes.Add(
                    EntityChange<TEntityId, TData>.Remove(
                        id,
                        previousData));
            }

            PublishChanged(
                EntityChangeSource.Batch,
                changes);
        }

        protected virtual void OnChanged(
            EntityChangeSet<TEntityId, TData> changes)
        {
        }

        protected override void OnDispose()
        {
            _changed.Dispose();
            _countChanged.Dispose();

            foreach (var subject in _changedById.Values)
            {
                subject.Dispose();
            }

            _changedById.Clear();

            base.OnDispose();
        }

        private void PublishChanged(
            EntityChangeSource source,
            IReadOnlyList<EntityChange<TEntityId, TData>> changes)
        {
            var changeSet = new EntityChangeSet<TEntityId, TData>(
                source,
                changes);

            if (changeSet.IsEmpty)
            {
                return;
            }

            _changed.OnNext(changeSet);

            var affectsCount = false;

            foreach (var change in changeSet)
            {
                PublishEntityChange(change);

                if (change.Operation == EntityChangeOperation.Added ||
                    change.Operation == EntityChangeOperation.Removed)
                {
                    affectsCount = true;
                }
            }

            if (affectsCount)
            {
                _countChanged.OnNext(_items.Count);
            }

            OnChanged(changeSet);
        }

        private void PublishChanged(
            EntityChangeSource source,
            EntityChange<TEntityId, TData> change)
        {
            var changeSet = new EntityChangeSet<TEntityId, TData>(
                source,
                change);

            _changed.OnNext(changeSet);

            PublishEntityChange(change);

            if (change.Operation == EntityChangeOperation.Added ||
                change.Operation == EntityChangeOperation.Removed)
            {
                _countChanged.OnNext(_items.Count);
            }

            OnChanged(changeSet);
        }

        private void PublishEntityChange(
            EntityChange<TEntityId, TData> change)
        {
            if (_changedById.TryGetValue(change.Id, out var subject))
            {
                subject.OnNext(change);
            }
        }

        private static IEnumerable<T> EmptyIfNull<T>(IEnumerable<T> items)
        {
            return items ?? Enumerable.Empty<T>();
        }

        private static IReadOnlyList<KeyValuePair<TEntityId, TData>> MaterializeDistinctItems(
            IEnumerable<KeyValuePair<TEntityId, TData>> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            var result = new List<KeyValuePair<TEntityId, TData>>();
            var ids = new HashSet<TEntityId>();

            foreach (var item in items)
            {
                if (!ids.Add(item.Key))
                {
                    throw new InvalidOperationException(
                        $"Entity is duplicated in items: {typeof(TEntityId).Name} = {item.Key}");
                }

                result.Add(item);
            }

            return result;
        }

        private void ValidateBatch(EntityBatch<TEntityId, TData> batch)
        {
            var existingIds = new HashSet<TEntityId>(_items.Keys);
            var addIds = new HashSet<TEntityId>();
            var updateIds = new HashSet<TEntityId>();
            var removeIds = new HashSet<TEntityId>();

            foreach (var item in EmptyIfNull(batch.Adds))
            {
                if (!addIds.Add(item.Key))
                {
                    throw new InvalidOperationException(
                        $"Entity is duplicated in batch adds: {typeof(TEntityId).Name} = {item.Key}");
                }

                if (existingIds.Contains(item.Key))
                {
                    throw new InvalidOperationException(
                        $"Entity already exists: {typeof(TEntityId).Name} = {item.Key}");
                }
            }

            foreach (var item in EmptyIfNull(batch.Updates))
            {
                if (!updateIds.Add(item.Key))
                {
                    throw new InvalidOperationException(
                        $"Entity is duplicated in batch updates: {typeof(TEntityId).Name} = {item.Key}");
                }

                if (!existingIds.Contains(item.Key))
                {
                    throw new KeyNotFoundException(
                        $"Entity is not found: {typeof(TEntityId).Name} = {item.Key}");
                }

                if (addIds.Contains(item.Key))
                {
                    throw new InvalidOperationException(
                        $"Entity cannot be added and updated in the same batch: {typeof(TEntityId).Name} = {item.Key}");
                }
            }

            foreach (var id in EmptyIfNull(batch.Removes))
            {
                if (!removeIds.Add(id))
                {
                    continue;
                }

                if (addIds.Contains(id))
                {
                    throw new InvalidOperationException(
                        $"Entity cannot be added and removed in the same batch: {typeof(TEntityId).Name} = {id}");
                }

                if (updateIds.Contains(id))
                {
                    throw new InvalidOperationException(
                        $"Entity cannot be updated and removed in the same batch: {typeof(TEntityId).Name} = {id}");
                }
            }
        }
    }
}
