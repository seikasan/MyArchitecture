using System.Linq;
using UnityEngine;
using VContainer;

namespace MyArchitecture.Core
{
    public sealed class EntityViewFactory<TEntityId, TView> :
        EntityViewFactoryBase<TEntityId, TView>
        where TEntityId : notnull
        where TView : Component, IEntityView<TEntityId>
    {
        public EntityViewFactory(
            IObjectResolver resolver,
            EntityViewRegistry<TEntityId, TView> registry) : base(resolver, registry)
        {
        }

        public override TView Create(
            TView prefab,
            TEntityId id,
            Vector3 position,
            Quaternion rotation,
            Transform parent = null,
            bool register = true)
        {
            ThrowIfAlreadyRegistered(id, register);

            var view = Object.Instantiate(prefab, position, rotation, parent);

            try
            {
                Resolver.Inject(view);
                BindOrRegister(view, id, register);

                return view;
            }
            catch
            {
                Object.Destroy(view.gameObject);
                throw;
            }
        }

        public override TView Create(
            TView prefab,
            TEntityId id,
            Transform parent,
            bool worldPositionStays = false,
            bool register = true)
        {
            ThrowIfAlreadyRegistered(id, register);

            var view = Object.Instantiate(
                prefab,
                parent,
                worldPositionStays);

            try
            {
                Resolver.Inject(view);
                BindOrRegister(view, id, register);

                return view;
            }
            catch
            {
                Object.Destroy(view.gameObject);
                throw;
            }
        }

        public void Destroy(TView view)
        {
            if (view == null) return;

            if (!Registry.Unregister(view))
            {
                view.UnbindEntity();
            }

            Object.Destroy(view.gameObject);
        }

        public void Destroy(TEntityId id)
        {
            if (!Registry.TryGet(id, out var view)) return;

            Registry.Unregister(id);

            Object.Destroy(view.gameObject);
        }

        public int DestroyAllRegisteredViews()
        {
            var views = Registry.Views.Values.ToArray();

            foreach (var view in views)
            {
                Destroy(view);
            }

            return views.Length;
        }
    }
}
