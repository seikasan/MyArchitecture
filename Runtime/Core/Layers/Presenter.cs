using VContainer;

namespace MyArchitecture.Core
{
    public abstract class Presenter :
        ArchitectureObject,
        IPresenter
    {
        public ICommandRunner CommandRunner { get; private set; }
        public IQueryRunner QueryRunner { get; private set; }
        public IEventSubscriber EventSubscriber { get; private set; }

        [Inject]
        private void InjectArchitectureDependencies(
            ICommandRunner commandRunner,
            IQueryRunner queryRunner,
            IEventSubscriber eventSubscriber)
        {
            CommandRunner = commandRunner;
            QueryRunner = queryRunner;
            EventSubscriber = eventSubscriber;
        }
    }
}
