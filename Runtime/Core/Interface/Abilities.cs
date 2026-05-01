using System;
using System.Threading;

namespace MyArchitecture.Core
{
    public interface IHasArchitectureLifetime
    {
        CancellationToken DisposeCancellationToken { get; }

        T Track<T>(T disposable)
            where T : IDisposable;
    }

    public interface ICanSendCommand : IHasArchitectureLifetime
    {
        ICommandRunner CommandRunner { get; }
    }

    public interface ICanSendQuery : IHasArchitectureLifetime
    {
        IQueryRunner QueryRunner { get; }
    }

    public interface ICanPublishEvent
    {
        IEventPublisher EventPublisher { get; }
    }

    public interface ICanSubscribeEvent : IHasArchitectureLifetime
    {
        IEventSubscriber EventSubscriber { get; }
    }

    public interface ICanExposeViewSignal
    {
    }

    public interface ICanUseTween
    {
    }
}
