using VContainer;

namespace MyArchitecture.Core
{
    public abstract class QueryBase :
        ArchitectureObject,
        ICanSendQuery
    {
        public IQueryRunner QueryRunner { get; private set; }

        [Inject]
        private void InjectArchitectureDependencies(IQueryRunner queryRunner)
        {
            QueryRunner = queryRunner;
        }
    }
}
