using System;
using System.Collections.Generic;

namespace MyArchitecture.Core
{
    public enum EntityChangeSource
    {
        Single,
        Batch,
        Clear
    }

    public enum EntityChangeOperation
    {
        Added,
        Updated,
        Removed
    }

    public readonly struct EntityChange<TEntityId, TData>
    {
        private EntityChange(
            EntityChangeOperation operation,
            TEntityId id,
            TData previousData,
            TData currentData)
        {
            Operation = operation;
            Id = id;
            PreviousData = previousData;
            CurrentData = currentData;
        }

        public EntityChangeOperation Operation { get; }
        public TEntityId Id { get; }

        public TData PreviousData { get; }
        public TData CurrentData { get; }

        public bool HasPreviousData => Operation != EntityChangeOperation.Added;
        public bool HasCurrentData => Operation != EntityChangeOperation.Removed;

        public static EntityChange<TEntityId, TData> Add(
            TEntityId id,
            TData currentData)
        {
            return new EntityChange<TEntityId, TData>(
                EntityChangeOperation.Added,
                id,
                default,
                currentData);
        }

        public static EntityChange<TEntityId, TData> Update(
            TEntityId id,
            TData previousData,
            TData currentData)
        {
            return new EntityChange<TEntityId, TData>(
                EntityChangeOperation.Updated,
                id,
                previousData,
                currentData);
        }

        public static EntityChange<TEntityId, TData> Remove(
            TEntityId id,
            TData previousData)
        {
            return new EntityChange<TEntityId, TData>(
                EntityChangeOperation.Removed,
                id,
                previousData,
                default);
        }
    }

    public readonly struct EntityChangeSet<TEntityId, TData>
    {
        private readonly EntityChange<TEntityId, TData> _singleChange;
        private readonly IReadOnlyList<EntityChange<TEntityId, TData>> _changes;

        public EntityChangeSet(
            EntityChangeSource source,
            EntityChange<TEntityId, TData> singleChange)
        {
            Source = source;
            _singleChange = singleChange;
            _changes = null;
            Count = 1;
        }

        public EntityChangeSet(
            EntityChangeSource source,
            IReadOnlyList<EntityChange<TEntityId, TData>> changes)
        {
            Source = source;
            _singleChange = default;
            _changes = changes ?? Array.Empty<EntityChange<TEntityId, TData>>();
            Count = _changes.Count;
        }

        public EntityChangeSource Source { get; }
        public int Count { get; }

        public bool IsEmpty => Count == 0;
        public bool IsSingle => Source == EntityChangeSource.Single;
        public bool IsBatch => Source == EntityChangeSource.Batch;
        public bool IsClear => Source == EntityChangeSource.Clear;

        public EntityChange<TEntityId, TData> this[int index]
        {
            get
            {
                if (_changes != null)
                {
                    return _changes[index];
                }

                if (index == 0)
                {
                    return _singleChange;
                }

                throw new ArgumentOutOfRangeException(nameof(index));
            }
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        public struct Enumerator
        {
            private readonly EntityChangeSet<TEntityId, TData> _changeSet;
            private int _index;

            public Enumerator(EntityChangeSet<TEntityId, TData> changeSet)
            {
                _changeSet = changeSet;
                _index = -1;
            }

            public EntityChange<TEntityId, TData> Current
                => _changeSet[_index];

            public bool MoveNext()
            {
                _index++;
                return _index < _changeSet.Count;
            }
        }
    }

    public readonly struct EntityViewRegisteredArgs<TEntityId, TView>
    {
        public EntityViewRegisteredArgs(TEntityId id, TView view)
        {
            Id = id;
            View = view;
        }

        public TEntityId Id { get; }
        public TView View { get; }
    }

    public readonly struct EntityViewUnregisteredArgs<TEntityId, TView>
    {
        public EntityViewUnregisteredArgs(TEntityId id, TView view)
        {
            Id = id;
            View = view;
        }

        public TEntityId Id { get; }
        public TView View { get; }
    }
}
