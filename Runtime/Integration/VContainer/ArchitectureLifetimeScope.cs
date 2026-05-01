using MessagePipe;
using MyArchitecture.Core;
using MyArchitecture.Unity;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace MyArchitecture.Integration
{
    public abstract class ArchitectureLifetimeScope : LifetimeScope
    {
        protected virtual Lifetime DefaultLifetime => Lifetime.Scoped;

        protected virtual ArchitectureSettings CreateSettings()
        {
            return ArchitectureSettings.Default();
        }

        protected virtual void RegisterLogger(IContainerBuilder builder)
        {
            builder.Register<UnityArchitectureLogger>(Lifetime.Singleton)
                .As<IArchitectureLogger>();
        }

        protected virtual void RegisterSceneLoader(IContainerBuilder builder)
        {
            builder.Register<UnitySceneLoader>(Lifetime.Singleton)
                .As<ISceneLoader>();
        }

        protected sealed override void Configure(IContainerBuilder builder)
        {
            var settings = CreateSettings();
            var commandRegistry = new CommandRegistry();
            var queryRegistry = new QueryRegistry();

            builder.RegisterInstance(settings)
                .AsSelf();

            builder.RegisterInstance(commandRegistry)
                .AsSelf();

            builder.RegisterInstance(queryRegistry)
                .AsSelf();

            RegisterLogger(builder);
            RegisterSceneLoader(builder);

            var messagePipeOptions = RegisterArchitectureCore(builder);

            var context = new ArchitectureRegistrationContext(
                builder,
                messagePipeOptions,
                DefaultLifetime,
                commandRegistry,
                queryRegistry);

            RegisterViews(context);
            AutoRegisterSceneViews(context);
            RegisterModels(context);
            RegisterGameServices(context);
            RegisterPresenters(context);
            RegisterCommands(context);
            RegisterQueries(context);
            RegisterUtilities(context);
            RegisterEvents(context);
            InstallFeatures(context);

            ArchitecturePackageAutoRegistration.Register(context);
            ArchitectureAutoRegistration.Register(context);

            RegisterArchitectureInitializer(builder);

            OnConfigured(context);
        }

        protected virtual void RegisterViews(
            ArchitectureRegistrationContext context)
        {
        }

        private void AutoRegisterSceneViews(
            ArchitectureRegistrationContext context)
        {
            var behaviours = Object.FindObjectsByType<MonoBehaviour>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None);

            foreach (var behaviour in behaviours)
            {
                if (behaviour == null ||
                    behaviour is not IView view)
                {
                    continue;
                }

                context.RegisterSceneView(view);
            }
        }

        protected virtual void RegisterModels(
            ArchitectureRegistrationContext context)
        {
        }

        protected virtual void RegisterPresenters(
            ArchitectureRegistrationContext context)
        {
        }

        protected virtual void RegisterGameServices(
            ArchitectureRegistrationContext context)
        {
        }

        protected virtual void RegisterCommands(
            ArchitectureRegistrationContext context)
        {
        }

        protected virtual void RegisterQueries(
            ArchitectureRegistrationContext context)
        {
        }

        protected virtual void RegisterEvents(
            ArchitectureRegistrationContext context)
        {
        }

        protected virtual void RegisterUtilities(
            ArchitectureRegistrationContext context)
        {
        }

        protected virtual void InstallFeatures(
            ArchitectureRegistrationContext context)
        {
        }

        protected virtual void OnConfigured(
            ArchitectureRegistrationContext context)
        {
        }

        private MessagePipeOptions RegisterArchitectureCore(
            IContainerBuilder builder)
        {
            MessagePipeOptions messagePipeOptions = builder.RegisterMessagePipe();

            builder.Register<QueryRunner>(Lifetime.Scoped)
                .As<IQueryRunner>();

            builder.Register<CommandRunner>(Lifetime.Scoped)
                .As<ICommandRunner>();

            builder.Register<MessagePipeEventPublisher>(Lifetime.Singleton)
                .As<IEventPublisher>();

            builder.Register<MessagePipeEventSubscriber>(Lifetime.Singleton)
                .As<IEventSubscriber>();

            return messagePipeOptions;
        }

        private void RegisterArchitectureInitializer(
            IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<ArchitectureInitializer>();
        }
    }
}
