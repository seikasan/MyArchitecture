using System;
using UnityEngine;
using VContainer;

namespace MyArchitecture.Core
{
    public abstract class EntityViewFactoryBase<TEntityId, TView>
        where TEntityId : notnull
        where TView : Component, IEntityView<TEntityId>
    {
        protected IObjectResolver Resolver { get; private set; }
        protected EntityViewRegistry<TEntityId, TView> Registry { get; private set; }

        protected EntityViewFactoryBase(
            IObjectResolver resolver,
            EntityViewRegistry<TEntityId, TView> registry)
        {
            Resolver = resolver;
            Registry = registry;
        }

        public abstract TView Create(
            TView prefab,
            TEntityId id,
            Vector3 position,
            Quaternion rotation,
            Transform parent = null,
            bool register = true);

        public abstract TView Create(
            TView prefab,
            TEntityId id,
            Transform parent,
            bool worldPositionStays = false,
            bool register = true);

        protected void ThrowIfAlreadyRegistered(TEntityId id, bool register)
        {
            if (!register ||
                !Registry.Contains(id))
            {
                return;
            }

            throw new InvalidOperationException(
                $"View is already registered: {typeof(TEntityId).Name} = {id}");
        }

        protected void BindOrRegister(TView view, TEntityId id, bool register)
        {
            if (!register)
            {
                view.BindEntity(id);
                return;
            }

            Registry.Register(id, view);
        }
    }
}