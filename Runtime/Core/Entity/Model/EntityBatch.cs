using System.Collections.Generic;

namespace MyArchitecture.Core
{
    public readonly struct EntityBatch<TEntityId, TData>
    {
        public EntityBatch(
            IEnumerable<KeyValuePair<TEntityId, TData>> adds = null,
            IEnumerable<KeyValuePair<TEntityId, TData>> updates = null,
            IEnumerable<TEntityId> removes = null)
        {
            Adds = adds;
            Updates = updates;
            Removes = removes;
        }

        public IEnumerable<KeyValuePair<TEntityId, TData>> Adds { get; }
        public IEnumerable<KeyValuePair<TEntityId, TData>> Updates { get; }
        public IEnumerable<TEntityId> Removes { get; }
    }
}