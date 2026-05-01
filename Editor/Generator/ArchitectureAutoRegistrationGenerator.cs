using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MyArchitecture.Core;
using UnityEditor;

namespace MyArchitecture.Editor
{
    [InitializeOnLoad]
    internal static class ArchitectureAutoRegistrationGenerator
    {
        private const string ApplicationOutputPath =
            "Assets/Generated/MyArchitecture/ArchitectureAutoRegistration.g.cs";

        private const string PackageOutputPath =
            "Packages/com.seikasan.myarchitecture/Runtime/Generated/Package/ArchitecturePackageAutoRegistration.g.cs";

        private const string FrameworkRootPath =
            "Packages/com.seikasan.myarchitecture/";

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

        static ArchitectureAutoRegistrationGenerator()
        {
            EditorApplication.delayCall += GenerateWhenReady;
        }

        [MenuItem("Tools/MyArchitecture/Generate/Auto Registration")]
        public static void Generate()
        {
            bool applicationChanged = Generate(
                ApplicationOutputPath,
                "ArchitectureAutoRegistration",
                IsApplicationRuntimeConcreteClass,
                IsApplicationRuntimeConcreteType);

            bool packageChanged = Generate(
                PackageOutputPath,
                "ArchitecturePackageAutoRegistration",
                IsFrameworkRuntimeConcreteClass,
                IsFrameworkRuntimeConcreteType);

            UnityEngine.Debug.Log(applicationChanged || packageChanged ?
                "Architecture auto registration generated." :
                "Architecture auto registration is already up to date.");
        }

        private static bool Generate(
            string outputPath,
            string className,
            Func<Type, bool> classFilter,
            Func<Type, bool> typeFilter)
        {
            var registrations = new List<Registration>();

            AddRegistrations(
                registrations,
                TypeCache.GetTypesDerivedFrom<Model>(),
                "RegisterModel",
                classFilter);
            AddRegistrations(
                registrations,
                TypeCache.GetTypesDerivedFrom<GameService>(),
                "RegisterGameService",
                classFilter);
            AddRegistrations(
                registrations,
                TypeCache.GetTypesDerivedFrom<Presenter>(),
                "RegisterPresenter",
                classFilter);
            AddRegistrations(
                registrations,
                GetCommandTypes(),
                "RegisterCommand",
                classFilter);
            AddRegistrations(
                registrations,
                GetQueryTypes(),
                "RegisterQuery",
                classFilter);
            AddRegistrations(
                registrations,
                TypeCache.GetTypesDerivedFrom<IUtility>(),
                "RegisterUtility",
                classFilter);
            AddRegistrations(
                registrations,
                TypeCache.GetTypesDerivedFrom<IEvent>(),
                "RegisterEvent",
                typeFilter);

            var entityViewRegistrations = GetEntityViewRegistrations(classFilter)
                .ToArray();

            string source = BuildSource(
                className,
                registrations,
                entityViewRegistrations);

            return WriteIfChanged(outputPath, source);
        }

        private static void GenerateWhenReady()
        {
            if (EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                EditorApplication.delayCall += GenerateWhenReady;
                return;
            }

            Generate();
        }

        private static void AddRegistrations(
            List<Registration> registrations,
            IEnumerable<Type> types,
            string methodName,
            Func<Type, bool> typeFilter = null)
        {
            typeFilter ??= IsRuntimeConcreteClass;

            foreach (var type in types.Where(typeFilter))
            {
                registrations.Add(new Registration(methodName, type));
            }
        }

        private static bool IsRuntimeConcreteClass(Type type)
        {
            return type.IsClass &&
                   !type.IsAbstract &&
                   IsRuntimeConcreteType(type);
        }

        private static bool IsApplicationRuntimeConcreteClass(Type type)
        {
            return type.IsClass &&
                   !type.IsAbstract &&
                   IsApplicationRuntimeConcreteType(type);
        }

        private static bool IsFrameworkRuntimeConcreteClass(Type type)
        {
            return type.IsClass &&
                   !type.IsAbstract &&
                   IsFrameworkRuntimeConcreteType(type);
        }

        private static bool IsApplicationRuntimeConcreteType(Type type)
        {
            return IsRuntimeConcreteType(type) &&
                   !IsFrameworkOwnedType(type);
        }

        private static bool IsFrameworkRuntimeConcreteType(Type type)
        {
            return IsRuntimeConcreteType(type) &&
                   IsFrameworkOwnedType(type);
        }

        private static bool IsRuntimeConcreteType(Type type)
        {
            if (type.IsInterface ||
                type.IsAbstract ||
                type.IsNested ||
                type.IsGenericTypeDefinition ||
                type.ContainsGenericParameters)
            {
                return false;
            }

            if (!type.IsClass &&
                (!type.IsValueType || type.IsEnum))
            {
                return false;
            }

            var scriptPath = FindScriptPath(type);
            return IsRuntimeScriptPath(scriptPath);
        }

        private static bool IsFrameworkOwnedType(Type type)
        {
            return IsFrameworkOwnedPath(FindScriptPath(type));
        }

        private static bool IsFrameworkOwnedPath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return false;
            }

            var normalized = path.Replace('\\', '/');

            return normalized.StartsWith(
                FrameworkRootPath,
                StringComparison.Ordinal);
        }

        private static bool IsRuntimeScriptPath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return false;
            }

            var normalized = path.Replace('\\', '/');

            return (normalized.StartsWith("Assets/", StringComparison.Ordinal) ||
                    normalized.StartsWith(FrameworkRootPath, StringComparison.Ordinal)) &&
                   !normalized.Contains("/Editor/");
        }

        private static string FindScriptPath(Type type)
        {
            var guids = AssetDatabase.FindAssets($"{type.Name} t:Script");

            foreach (string guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var script = AssetDatabase.LoadAssetAtPath<MonoScript>(path);

                if (script == null)
                {
                    continue;
                }

                if (script.GetClass() == type ||
                    Path.GetFileNameWithoutExtension(path) == type.Name)
                {
                    return path;
                }
            }

            return null;
        }

        private static IEnumerable<Type> GetCommandTypes()
        {
            return GetTypesDerivedFrom(CommandInterfaceTypes);
        }

        private static IEnumerable<Type> GetQueryTypes()
        {
            return GetTypesDerivedFrom(QueryInterfaceTypes);
        }

        private static IEnumerable<Type> GetTypesDerivedFrom(IEnumerable<Type> parentTypes)
        {
            foreach (var parentType in parentTypes)
            {
                foreach (var type in TypeCache.GetTypesDerivedFrom(parentType))
                {
                    yield return type;
                }
            }
        }

        private static IEnumerable<EntityViewRegistration> GetEntityViewRegistrations(
            Func<Type, bool> typeFilter)
        {
            foreach (var viewType in TypeCache.GetTypesDerivedFrom<IView>())
            {
                if (!typeFilter(viewType))
                {
                    continue;
                }

                if (!typeof(UnityEngine.Component).IsAssignableFrom(viewType))
                {
                    UnityEngine.Debug.LogWarning(
                        $"ArchitectureAutoRegistration skipped entity view {viewType.FullName}: " +
                        "entity view must inherit UnityEngine.Component.");

                    continue;
                }

                foreach (var registration in GetEntityViewRegistrations(viewType))
                {
                    yield return registration;
                }
            }
        }

        private static IEnumerable<EntityViewRegistration> GetEntityViewRegistrations(
            Type viewType)
        {
            var sceneEntityIdTypes = GetSceneEntityIdTypes(viewType)
                .ToArray();

            if (sceneEntityIdTypes.Length > 0)
            {
                foreach (var entityIdType in sceneEntityIdTypes)
                {
                    yield return new EntityViewRegistration(
                        entityIdType,
                        GetSceneEntityRegistryViewType(viewType, entityIdType),
                        registerFactories: false);
                }

                yield break;
            }

            foreach (var entityIdType in GetEntityIdTypes(viewType))
            {
                yield return new EntityViewRegistration(
                    entityIdType,
                    viewType,
                    registerFactories: true);
            }
        }

        private static IEnumerable<Type> GetEntityIdTypes(Type viewType)
        {
            foreach (var interfaceType in viewType.GetInterfaces())
            {
                if (!interfaceType.IsGenericType)
                {
                    continue;
                }

                if (interfaceType.GetGenericTypeDefinition() != typeof(IEntityView<>))
                {
                    continue;
                }

                yield return interfaceType.GetGenericArguments()[0];
            }
        }

        private static IEnumerable<Type> GetSceneEntityIdTypes(Type viewType)
        {
            foreach (var interfaceType in viewType.GetInterfaces())
            {
                if (!interfaceType.IsGenericType)
                {
                    continue;
                }

                if (interfaceType.GetGenericTypeDefinition() != typeof(ISceneEntityView<>))
                {
                    continue;
                }

                yield return interfaceType.GetGenericArguments()[0];
            }
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

        private static string BuildSource(
            string className,
            IEnumerable<Registration> registrations,
            IEnumerable<EntityViewRegistration> entityViewRegistrations)
        {
            var builder = new StringBuilder();
            builder.AppendLine("// <auto-generated />");
            builder.AppendLine("// This file is generated by ArchitectureAutoRegistrationGenerator.");
            builder.AppendLine();
            builder.AppendLine("namespace MyArchitecture.Integration");
            builder.AppendLine("{");
            builder.Append("    internal static class ")
                .AppendLine(className);
            builder.AppendLine("    {");
            builder.AppendLine("        public static void Register(ArchitectureRegistrationContext context)");
            builder.AppendLine("        {");

            foreach (var registration in registrations
                         .Distinct()
                         .OrderBy(registration => registration.MethodOrder)
                         .ThenBy(registration => registration.Type.FullName))
            {
                builder.Append("            context.")
                    .Append(registration.MethodName)
                    .Append('<')
                    .Append(GetTypeName(registration.Type))
                    .AppendLine(">();");
            }

            foreach (var registration in entityViewRegistrations
                         .GroupBy(registration => new
                         {
                             registration.EntityIdType,
                             registration.ViewType
                         })
                         .Select(group => new EntityViewRegistration(
                             group.Key.EntityIdType,
                             group.Key.ViewType,
                             group.Any(registration => registration.RegisterFactories)))
                         .OrderBy(registration => registration.ViewType.FullName))
            {
                builder.Append("            context.RegisterEntityViewRegistry<")
                    .Append(GetTypeName(registration.EntityIdType))
                    .Append(", ")
                    .Append(GetTypeName(registration.ViewType))
                    .AppendLine(">();");

                if (!registration.RegisterFactories)
                {
                    continue;
                }

                builder.Append("            context.RegisterEntityViewFactory<")
                    .Append(GetTypeName(registration.EntityIdType))
                    .Append(", ")
                    .Append(GetTypeName(registration.ViewType))
                    .AppendLine(">();");

                builder.Append("            context.RegisterPooledEntityViewFactory<")
                    .Append(GetTypeName(registration.EntityIdType))
                    .Append(", ")
                    .Append(GetTypeName(registration.ViewType))
                    .AppendLine(">();");
            }

            builder.AppendLine("        }");
            builder.AppendLine("    }");
            builder.AppendLine("}");

            return builder.ToString();
        }

        private static string GetTypeName(Type type)
        {
            var fullName = type.FullName ?? type.Name;
            return "global::" + fullName.Replace('+', '.');
        }

        private static bool WriteIfChanged(
            string outputPath,
            string source)
        {
            var directory = Path.GetDirectoryName(outputPath);

            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            if (File.Exists(outputPath) &&
                File.ReadAllText(outputPath) == source)
            {
                return false;
            }

            File.WriteAllText(outputPath, source, Encoding.UTF8);
            AssetDatabase.ImportAsset(outputPath);

            return true;
        }

        private readonly struct Registration : IEquatable<Registration>
        {
            public Registration(string methodName, Type type)
            {
                MethodName = methodName;
                Type = type;
            }

            public string MethodName { get; }

            public Type Type { get; }

            public int MethodOrder
            {
                get
                {
                    switch (MethodName)
                    {
                        case "RegisterModel":
                            return 0;
                        case "RegisterGameService":
                            return 1;
                        case "RegisterPresenter":
                            return 2;
                        case "RegisterCommand":
                            return 3;
                        case "RegisterQuery":
                            return 4;
                        case "RegisterUtility":
                            return 5;
                        case "RegisterEvent":
                            return 6;
                        default:
                            return int.MaxValue;
                    }
                }
            }

            public bool Equals(Registration other)
            {
                return MethodName == other.MethodName &&
                       Type == other.Type;
            }

            public override bool Equals(object obj)
            {
                return obj is Registration other && Equals(other);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return ((MethodName != null ? MethodName.GetHashCode() : 0) * 397) ^
                           (Type != null ? Type.GetHashCode() : 0);
                }
            }
        }

        private readonly struct EntityViewRegistration :
            IEquatable<EntityViewRegistration>
        {
            public EntityViewRegistration(
                Type entityIdType,
                Type viewType,
                bool registerFactories)
            {
                EntityIdType = entityIdType;
                ViewType = viewType;
                RegisterFactories = registerFactories;
            }

            public Type EntityIdType { get; }

            public Type ViewType { get; }

            public bool RegisterFactories { get; }

            public bool Equals(EntityViewRegistration other)
            {
                return EntityIdType == other.EntityIdType &&
                       ViewType == other.ViewType &&
                       RegisterFactories == other.RegisterFactories;
            }

            public override bool Equals(object obj)
            {
                return obj is EntityViewRegistration other && Equals(other);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    var hashCode = EntityIdType != null ? EntityIdType.GetHashCode() : 0;
                    hashCode = (hashCode * 397) ^ (ViewType != null ? ViewType.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ RegisterFactories.GetHashCode();
                    return hashCode;
                }
            }
        }
    }
}
