namespace MyArchitecture.Core
{
    public interface IPooledEntityView<TEntityId>
        where TEntityId : notnull
    {
        void OnRentFromPool(TEntityId id);
        void OnReturnToPool();
    }
}