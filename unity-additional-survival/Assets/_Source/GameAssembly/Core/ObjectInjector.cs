using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Core
{
    public static class ObjectInjector
    {
        private static IObjectResolver _resolver;
        
        public static void InitInjector(IObjectResolver resolver) => _resolver = resolver;

        public static void InjectGameObject(GameObject go) => _resolver.InjectGameObject(go);
        public static void InjectObject(object instance) => _resolver.Inject(instance);
    }
}