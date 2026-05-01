namespace MyArchitecture.Core
{
    public interface IEntityView<TEntityId> :
        IView
        where TEntityId : notnull
    {
        TEntityId EntityId { get; }

        bool HasEntity { get; }

        void BindEntity(TEntityId id);

        void UnbindEntity();
    }
}
