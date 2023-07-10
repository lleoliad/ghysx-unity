using Loxodon.Framework.Binding.Reflection;
using System;
using System.Reflection;
using GhysX.Framework.Anims;
using UnityEngine;
using UnityEngine.UI;

namespace Loxodon.Framework.Binding
{
    public class UnityProxyRegisterEx
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Initialize()
        {
            Register<AnimationListener, AnimationListener.AnimationEvent>("onEvent", t => t.onEvent, null);
        }
        
        public static void Register<T, TValue>(string name, Func<T, TValue> getter, Action<T, TValue> setter)
        {
            var propertyInfo = typeof(T).GetProperty(name);
            if (propertyInfo is PropertyInfo)
            {
                ProxyFactory.Default.Register(new ProxyPropertyInfo<T, TValue>(name, getter, setter));
                return;
            }

            var fieldInfo = typeof(T).GetField(name);
            if (fieldInfo is FieldInfo)
            {
                ProxyFactory.Default.Register(new ProxyFieldInfo<T, TValue>(name, getter, setter));
                return;
            }

            throw new Exception(string.Format("Not found the property or field named '{0}' in {1} type", name, typeof(T).Name));
        }
    }
}