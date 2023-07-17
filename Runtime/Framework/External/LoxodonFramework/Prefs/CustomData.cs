using System;
using Loxodon.Framework.Prefs;
using UnityEngine;

namespace GhysX.Framework.External.Prefs
{
    public struct CustomData
    {
        public CustomData(string name, string description)
        {
            this.name = name;
            this.description = description;
        }

        public string name;
        public string description;
    }

    public class CustomDataTypeEncoder<T> : ITypeEncoder
    {
        private int priority = 0;

        public int Priority
        {
            get { return this.priority; }
            set { this.priority = value; }
        }

        public bool IsSupport(Type type)
        {
            return typeof(T).Equals(type);
        }

        public object Decode(Type type, string value)
        {
            return JsonUtility.FromJson(value, type);
        }

        public string Encode(object value)
        {
            return JsonUtility.ToJson(value);
        }
    }
}