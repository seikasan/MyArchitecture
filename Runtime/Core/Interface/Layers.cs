namespace MyArchitecture.Core
{
    public interface IPresenter :
        IArchitectureObject,
        ICanSendCommand,
        ICanSendQuery,
        ICanSubscribeEvent,
        IArchitectureInitializable
    {
    }

    public interface IView :
        IArchitectureObject,
        ICanExposeViewSignal,
        ICanUseTween
    {
    }

    public interface IGameService :
        IArchitectureObject,
        ICanSendQuery,
        ICanPublishEvent,
        IArchitectureInitializable
    {
    }

    public interface IModel :
        IArchitectureObject,
        IArchitectureInitializable
    {
    }

    public interface IReadOnlyModel :
        IArchitectureObject
    {
    }

    public interface IUtility :
        IArchitectureObject
    {
    }
}
