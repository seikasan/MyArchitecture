using VContainer;

namespace MyArchitecture.Core
{
    public abstract class CommandBase :
        ArchitectureObject,
        ICanSendCommand,
        ICanSendQuery,
        ICanPublishEvent
    {
        public ICommandRunner CommandRunner { get; private set; }
        public IQueryRunner QueryRunner { get; private set; }
        public IEventPublisher EventPublisher { get; private set; }

        [Inject]
        private void InjectArchitectureDependencies(
            ICommandRunner commandRunner,
            IQueryRunner queryRunner,
            IEventPublisher eventPublisher)
        {
            CommandRunner = commandRunner;
            QueryRunner = queryRunner;
            EventPublisher = eventPublisher;
        }
    }
}
