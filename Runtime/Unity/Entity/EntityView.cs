using MyArchitecture.Core;

namespace MyArchitecture.Unity
{
    public abstract class EntityView<TEntityId> :
        View,
        IEntityView<TEntityId>
        where TEntityId : notnull
    {
        public TEntityId EntityId { get; private set; }

        public bool HasEntity { get; private set; }

        public void BindEntity(TEntityId id)
        {
            EntityId = id;
            HasEntity = true;
            OnEntityBound(id);
        }

        public void UnbindEntity()
        {
            if (!HasEntity) return;

            var id = EntityId;

            EntityId = default;
            HasEntity = false;

            OnEntityUnbound(id);
        }

        protected virtual void OnEntityBound(TEntityId id)
        {
        }

        protected virtual void OnEntityUnbound(TEntityId id)
        {
        }
    }

    public abstract class EntityView<TEntityId, TData> :
        EntityView<TEntityId>,
        IEntityDataView<TData>
        where TEntityId : notnull
    {
        public abstract void Apply(TData data);
    }
}
