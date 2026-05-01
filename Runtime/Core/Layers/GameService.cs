using VContainer;

namespace MyArchitecture.Core
{
    public abstract class GameService :
        ArchitectureObject,
        IGameService
    {
        public IQueryRunner QueryRunner { get; private set; }
        public IEventPublisher EventPublisher { get; private set; }

        [Inject]
        private void InjectArchitectureDependencies(
            IQueryRunner queryRunner,
            IEventPublisher eventPublisher)
        {
            QueryRunner = queryRunner;
            EventPublisher = eventPublisher;
        }
    }
}
