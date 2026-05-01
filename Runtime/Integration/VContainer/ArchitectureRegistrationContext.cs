using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MessagePipe;
using MyArchitecture.Core;
using VContainer;

namespace MyArchitecture.Integration
{
    public sealed class ArchitectureRegistrationContext
    {
        private static readonly Type[] CommandInterfaceTypes =
        {
            typeof(ICommand),
            typeof(ICommand<>),
            typeof(ICommand<,>),
            typeof(ICommand<,,>),
            typeof(IAsyncCommand),
            typeof(IAsyncCommand<>),
            typeof(IAsyncCommand<,>),
            typeof(IAsyncCommand<,,>),
            typeof(ITryCommand),
            typeof(ITryCommand<>),
            typeof(ITryCommand<,>),
            typeof(ITryCommand<,,>),
            typeof(IAsyncTryCommand),
            typeof(IAsyncTryCommand<>),
            typeof(IAsyncTryCommand<,>),
            typeof(IAsyncTryCommand<,,>),
            typeof(IResultCommand<>),
            typeof(IResultCommand<,>),
            typeof(IResultCommand<,,>),
            typeof(IResultCommand<,,,>),
            typeof(IAsyncResultCommand<>),
            typeof(IAsyncResultCommand<,>),
            typeof(IAsyncResultCommand<,,>),
            typeof(IAsyncResultCommand<,,,>)
        };

        private static readonly Type[] QueryInterfaceTypes =
        {
            typeof(IQuery<>),
            typeof(IQuery<,>),
            typeof(IQuery<,,>),
            typeof(IQuery<,,,>),
            typeof(IAsyncQuery<>),
            typeof(IAsyncQuery<,>),
            typeof(IAsyncQuery<,,>),
            typeof(IAsyncQuery<,,,>)
        };

        private readonly HashSet<Type> _manualViewServiceTypes = new();
        private readonly Dictionary<Type, Type> _autoRegisteredViewServiceTypes = new();
        private readonly HashSet<Type> _registeredModelTypes = new();
        private readonly HashSet<Type> _registeredGameServiceTypes = new();
        private readonly HashSet<Type> _registeredPresenterTypes = new();
        private readonly HashSet<Type> _registeredUtilityTypes = new();
        private readonly HashSet<Type> _registeredCommandTypes = new();
        private readonly HashSet<Type> _registeredQueryTypes = new();
        private readonly HashSet<Type> _registeredEventTypes = new();
        private readonly HashSet<Type> _registeredEntityViewRegistryTypes = new();
        private readonly HashSet<Type> _registeredEntityViewFactoryTypes = new();
        private readonly HashSet<Type> _registeredPooledEntityViewFactoryTypes = new();

        public IContainerBuilder Builder { get; }
        public MessagePipeOptions MessagePipeOptions { get; }
        public Lifetime DefaultLifetime { get; }
        public CommandRegistry CommandRegistry { get; }
        public QueryRegistry QueryRegistry { get; }

        public ArchitectureRegistrationContext(
            IContainerBuilder builder,
            MessagePipeOptions messagePipeOptions,
            Lifetime defaultLifetime,
            CommandRegistry commandRegistry,
            QueryRegistry queryRegistry)
        {
            Builder = builder;
            MessagePipeOptions = messagePipeOptions;
            DefaultLifetime = defaultLifetime;
            CommandRegistry = commandRegistry;
            QueryRegistry = queryRegistry;
        }

        public void Install(IArchitectureInstaller installer)
        {
            installer.Install(this);
        }

        public void RegisterView<TViewInterface>(TViewInterface view)
            where TViewInterface : class, IView
        {
            if (view == null)
            {
                throw new ArgumentNullException(nameof(view));
            }

            if (!_manualViewServiceTypes.Add(typeof(TViewInterface)))
            {
                return;
            }

            Builder.RegisterInstance(view)
                .As<TViewInterface>();
        }

        public void RegisterSceneView(IView view)
        {
            if (view == null)
            {
                throw new ArgumentNullException(nameof(view));
            }

            var viewType = view.GetType();
            var sceneEntityViewInterface =
                GetSceneEntityViewInterface(viewType);

            if (sceneEntityViewInterface != null)
            {
                RegisterSceneEntityView(
                    view,
                    viewType,
                    sceneEntityViewInterface);

                return;
            }

            if (IsEntityViewType(viewType))
            {
                return;
            }

            var serviceTypes = GetSceneViewServiceTypes(viewType)
                .Where(type => !_manualViewServiceTypes.Contains(type))
                .ToArray();

            if (serviceTypes.Length == 0)
            {
                return;
            }

            foreach (var serviceType in serviceTypes)
            {
                if (!_autoRegisteredViewServiceTypes.TryGetValue(
                        serviceType,
                        out var registeredViewType))
                {
                    continue;
                }

                throw new InvalidOperationException(
                    $"Scene View service is duplicated: {serviceType.FullName}. " +
                    $"Registered: {registeredViewType.FullName}, Duplicate: {viewType.FullName}. " +
                    "Use EntityView for multiple instances, or split the View interface.");
            }

            foreach (var serviceType in serviceTypes)
            {
                _autoRegisteredViewServiceTypes.Add(serviceType, viewType);
            }

            Builder.RegisterInstance(view, viewType)
                .As(serviceTypes);

            Builder.RegisterBuildCallback(container => container.Inject(view));
        }

        private static readonly Dictionary<(Type, Type), Action<IObjectResolver, IView>>
            _sceneEntityViewRegisterCache = new();

        private void RegisterSceneEntityView(
            IView view,
            Type viewType,
            Type sceneEntityViewInterface)
        {
            var entityIdType = sceneEntityViewInterface
                .GetGenericArguments()[0];
            var registryViewType = GetSceneEntityRegistryViewType(
                viewType,
                entityIdType);
            var register = GetOrCreateSceneEntityViewRegister(
                entityIdType,
                registryViewType);

            Builder.RegisterBuildCallback(container =>
            {
                register(container, view);
            });
        }

        private static Action<IObjectResolver, IView> GetOrCreateSceneEntityViewRegister(
            Type entityIdType,
            Type registryViewType)
        {
            var key = (entityIdType, registryViewType);

            if (_sceneEntityViewRegisterCache.TryGetValue(key, out var cached))
            {
                return cached;
            }

            var method = typeof(ArchitectureRegistrationContext)
                .GetMethod(
                    nameof(RegisterSceneEntityViewCore),
                    BindingFlags.NonPublic | BindingFlags.Static)
                .MakeGenericMethod(entityIdType, registryViewType);
            var register = (Action<IObjectResolver, IView>)Delegate.CreateDelegate(
                typeof(Action<IObjectResolver, IView>),
                method);

            _sceneEntityViewRegisterCache.Add(key, register);
            return register;
        }

        public void RegisterModel<TModel>(
            Lifetime? lifetime = null)
            where TModel : class, IModel
        {
            if (!_registeredModelTypes.Add(typeof(TModel)))
            {
                return;
            }

            var readOnlyModelTypes = typeof(TModel)
                .GetInterfaces()
                .Where(type =>
                    type != typeof(IReadOnlyModel) &&
                    typeof(IReadOnlyModel).IsAssignableFrom(type))
                .ToArray();

            if (readOnlyModelTypes.Length == 0)
            {
                throw new InvalidOperationException(
                    $"Read-only model interface is not generated for {typeof(TModel).FullName}. " +
                    "Make the model partial and let ReadOnlyModelGenerator generate IReadOnly{ModelName}.");
            }

            Builder.Register<TModel>(lifetime ?? DefaultLifetime)
                .AsSelf()
                .As<IArchitectureInitializable>()
                .As(readOnlyModelTypes);
        }

        public void RegisterGameService<TGameService>(
            Lifetime? lifetime = null)
            where TGameService : class, IGameService, IArchitectureInitializable
        {
            if (!_registeredGameServiceTypes.Add(typeof(TGameService)))
            {
                return;
            }

            Builder.Register<TGameService>(lifetime ?? DefaultLifetime)
                .AsSelf()
                .As<IGameService>()
                .As<IArchitectureInitializable>();
        }

        public void RegisterPresenter<TPresenter>(
            Lifetime? lifetime = null)
            where TPresenter : class, IPresenter, IArchitectureInitializable
        {
            if (!_registeredPresenterTypes.Add(typeof(TPresenter)))
            {
                return;
            }

            Builder.Register<TPresenter>(lifetime ?? DefaultLifetime)
                .AsSelf()
                .As<IPresenter>()
                .As<IArchitectureInitializable>();
        }

        public void RegisterUtility<TUtility>(
            Lifetime? lifetime = null)
            where TUtility : class, IUtility
        {
            if (!_registeredUtilityTypes.Add(typeof(TUtility)))
            {
                return;
            }

            Builder.Register<TUtility>(lifetime ?? DefaultLifetime)
                .AsSelf()
                .AsImplementedInterfaces();
        }

        public void RegisterCommand<TCommand>()
            where TCommand : class
        {
            if (!IsOrImplements(typeof(TCommand), CommandInterfaceTypes))
            {
                throw new InvalidOperationException(
                    $"Registered command type does not implement a command interface: {typeof(TCommand).FullName}");
            }

            if (!_registeredCommandTypes.Add(typeof(TCommand)))
            {
                return;
            }

            CommandRegistry.Add<TCommand>();

            Builder.Register<TCommand>(DefaultLifetime)
                .AsSelf();
        }

        public void RegisterQuery<TQuery>()
            where TQuery : class
        {
            if (!IsOrImplements(typeof(TQuery), QueryInterfaceTypes))
            {
                throw new InvalidOperationException(
                    $"Registered query type does not implement a query interface: {typeof(TQuery).FullName}");
            }

            if (!_registeredQueryTypes.Add(typeof(TQuery)))
            {
                return;
            }

            QueryRegistry.Add<TQuery>();

            Builder.Register<TQuery>(DefaultLifetime)
                .AsSelf();
        }

        public void RegisterEvent<TEvent>()
            where TEvent : IEvent
        {
            if (!_registeredEventTypes.Add(typeof(TEvent)))
            {
                return;
            }

            Builder.RegisterMessageBroker<TEvent>(MessagePipeOptions);
        }

        public void RegisterEntityViewRegistry<TEntityId, TView>(
            Lifetime? lifetime = null)
            where TEntityId : notnull
            where TView : class, IEntityView<TEntityId>
        {
            var registryType = typeof(EntityViewRegistry<TEntityId, TView>);

            if (!_registeredEntityViewRegistryTypes.Add(registryType))
            {
                return;
            }

            Builder.Register<EntityViewRegistry<TEntityId, TView>>(lifetime ?? DefaultLifetime)
                .AsSelf();
        }

        public void RegisterEntityViewFactory<TEntityId, TView>(
            Lifetime? lifetime = null)
            where TEntityId : notnull
            where TView : UnityEngine.Component, IEntityView<TEntityId>
        {
            var factoryType = typeof(EntityViewFactory<TEntityId, TView>);

            if (!_registeredEntityViewFactoryTypes.Add(factoryType))
            {
                return;
            }

            Builder.Register<EntityViewFactory<TEntityId, TView>>(lifetime ?? DefaultLifetime)
                .AsSelf();
        }

        public void RegisterPooledEntityViewFactory<TEntityId, TView>(
            Lifetime? lifetime = null)
            where TEntityId : notnull
            where TView : UnityEngine.Component, IEntityView<TEntityId>
        {
            var factoryType = typeof(PooledEntityViewFactory<TEntityId, TView>);

            if (!_registeredPooledEntityViewFactoryTypes.Add(factoryType))
            {
                return;
            }

            Builder.Register<PooledEntityViewFactory<TEntityId, TView>>(
                    lifetime ?? DefaultLifetime)
                .AsSelf()
                .As<IEntityViewPool<TEntityId, TView>>();
        }

        private static IEnumerable<Type> GetSceneViewServiceTypes(Type viewType)
        {
            yield return viewType;

            foreach (var interfaceType in viewType.GetInterfaces()
                         .Where(IsSceneViewServiceInterface)
                         .OrderBy(type => type.FullName))
            {
                yield return interfaceType;
            }
        }

        private static bool IsSceneViewServiceInterface(Type type)
        {
            return type != typeof(IView) &&
                   type != typeof(ICanExposeViewSignal) &&
                   type != typeof(ICanUseTween) &&
                   typeof(IView).IsAssignableFrom(type) &&
                   !IsEntityViewInterface(type);
        }

        private static bool IsEntityViewType(Type type)
        {
            return type.GetInterfaces().Any(IsEntityViewInterface);
        }

        private static bool IsEntityViewInterface(Type type)
        {
            return type.IsGenericType &&
                   type.GetGenericTypeDefinition() == typeof(IEntityView<>);
        }

        private static Type GetSceneEntityViewInterface(Type viewType)
        {
            var interfaceTypes = viewType.GetInterfaces()
                .Where(IsSceneEntityViewInterface)
                .ToArray();

            if (interfaceTypes.Length > 1)
            {
                throw new InvalidOperationException(
                    $"{viewType.FullName} implements multiple ISceneEntityView<> interfaces.");
            }

            return interfaceTypes.Length == 1
                ? interfaceTypes[0]
                : null;
        }

        private static bool IsSceneEntityViewInterface(Type type)
        {
            return type.IsGenericType &&
                   type.GetGenericTypeDefinition() == typeof(ISceneEntityView<>);
        }

        private static Type GetSceneEntityRegistryViewType(
            Type viewType,
            Type entityIdType)
        {
            for (var current = viewType.BaseType; current != null; current = current.BaseType)
            {
                if (current.IsAbstract ||
                    current.IsGenericTypeDefinition ||
                    current.ContainsGenericParameters)
                {
                    continue;
                }

                if (!ImplementsEntityView(current, entityIdType) ||
                    ImplementsSceneEntityView(current, entityIdType))
                {
                    continue;
                }

                return current;
            }

            return viewType;
        }

        private static bool ImplementsEntityView(Type type, Type entityIdType)
        {
            return type.GetInterfaces().Any(interfaceType =>
                interfaceType.IsGenericType &&
                interfaceType.GetGenericTypeDefinition() == typeof(IEntityView<>) &&
                interfaceType.GetGenericArguments()[0] == entityIdType);
        }

        private static bool ImplementsSceneEntityView(Type type, Type entityIdType)
        {
            return type.GetInterfaces().Any(interfaceType =>
                interfaceType.IsGenericType &&
                interfaceType.GetGenericTypeDefinition() == typeof(ISceneEntityView<>) &&
                interfaceType.GetGenericArguments()[0] == entityIdType);
        }

        private static bool IsOrImplements(
            Type type,
            IEnumerable<Type> definitions)
        {
            foreach (var definition in definitions)
            {
                if (definition.IsGenericTypeDefinition)
                {
                    if (IsGenericDefinition(type, definition) ||
                        type.GetInterfaces().Any(interfaceType =>
                            IsGenericDefinition(interfaceType, definition)))
                    {
                        return true;
                    }

                    continue;
                }

                if (definition.IsAssignableFrom(type))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsGenericDefinition(Type type, Type definition)
        {
            return type.IsGenericType &&
                   type.GetGenericTypeDefinition() == definition;
        }

        private static void RegisterSceneEntityViewCore<TEntityId, TView>(
            IObjectResolver container,
            IView view)
            where TEntityId : notnull
            where TView : class, IEntityView<TEntityId>
        {
            var sceneView = (ISceneEntityView<TEntityId>)view;
            var registryView = (TView)view;

            container.Inject(view);

            container
                .Resolve<EntityViewRegistry<TEntityId, TView>>()
                .Register(sceneView.SceneEntityId, registryView);
        }
    }
}
