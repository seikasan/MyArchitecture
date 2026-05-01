using System.Collections.Generic;
using System.Linq;
using MyArchitecture.Core;
using VContainer.Unity;

namespace MyArchitecture.Integration
{
    public sealed class ArchitectureInitializer : IInitializable
    {
        private readonly IReadOnlyList<IArchitectureInitializable> _initializables;
        private readonly ArchitectureSettings _settings;
        private readonly IArchitectureLogger _logger;

        public ArchitectureInitializer(
            IEnumerable<IArchitectureInitializable> initializables,
            ArchitectureSettings settings,
            IArchitectureLogger logger)
        {
            _initializables = initializables
                .OrderBy(GetLayerRank)
                .ThenBy(initializable => initializable.InitializeOrder)
                .ThenBy(initializable => initializable.GetType().FullName)
                .ToArray();
            _settings = settings;
            _logger = logger;
        }

        public void Initialize()
        {
            if (_settings.EnableInitializationLog)
            {
                InitializeWithLog();
                return;
            }

            foreach (IArchitectureInitializable target in _initializables)
            {
                target.Initialize();
            }

            foreach (IArchitectureInitializable target in _initializables)
            {
                target.Bind();
            }

            foreach (IArchitectureInitializable target in _initializables)
            {
                target.PostInitialize();
            }
        }

        private void InitializeWithLog()
        {
            foreach (IArchitectureInitializable target in _initializables)
            {
                _logger.Log($"Initialize: {target.GetType().Name}");
                target.Initialize();
            }

            foreach (IArchitectureInitializable target in _initializables)
            {
                _logger.Log($"Bind: {target.GetType().Name}");
                target.Bind();
            }

            foreach (IArchitectureInitializable target in _initializables)
            {
                _logger.Log($"PostInitialize: {target.GetType().Name}");
                target.PostInitialize();
            }
        }

        private static int GetLayerRank(IArchitectureInitializable initializable)
        {
            return initializable switch
            {
                IModel => 0,
                IGameService => 1,
                IPresenter => 2,
                _ => 3
            };
        }
    }
}
