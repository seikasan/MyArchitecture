using System;
using System.Collections.Generic;
using R3;

namespace MyArchitecture.Core
{
    public interface IReadOnlyEntityCollectionModel<TEntityId, TData> :
        IReadOnlyModel
        where TEntityId : notnull
    {
        int Count { get; }
        Observable<int> CountChanged { get; }

        IEnumerable<TEntityId> Ids { get; }
        IEnumerable<KeyValuePair<TEntityId, TData>> Items { get; }

        Observable<EntityChangeSet<TEntityId, TData>> Changed { get; }
        Observable<EntityChange<TEntityId, TData>> ObserveEntity(TEntityId targetId);

        bool Contains(TEntityId id);
        bool Exists(TEntityId id);
        IReadOnlyList<KeyValuePair<TEntityId, TData>> Snapshot();

        bool TryGet(TEntityId id, out TData data);
        TData Get(TEntityId id);

        bool TryFindFirst(
            Func<TEntityId, TData, bool> predicate,
            out KeyValuePair<TEntityId, TData> result);

        List<KeyValuePair<TEntityId, TData>> FindAll(
            Func<TEntityId, TData, bool> predicate);
    }
}
