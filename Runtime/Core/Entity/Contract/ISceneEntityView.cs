namespace MyArchitecture.Core
{
    public interface ISceneEntityView<TEntityId> :
        IEntityView<TEntityId>
        where TEntityId : notnull
    {
        TEntityId SceneEntityId { get; }
    }
}