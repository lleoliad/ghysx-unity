using System;
using System.Collections.Generic;
using BestHTTP.JSON;
using Loxodon.Framework.Binding.Proxy.Targets;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace GhysX.Framework.Anims
{
    public class AnimationListener : MonoBehaviour
    {
        [Serializable]
        public class AnimationEvent : UnityEvent
        {
        }

        [FormerlySerializedAs("onEvent")] [SerializeField]
        private AnimationEvent m_OnEvent = new AnimationEvent();

        private object _parameter;

        public object Parameter => _parameter;

        public static AnimationListener Current { get; set; }

        public AnimationEvent onEvent
        {
            get { return m_OnEvent; }
            set { m_OnEvent = value; }
        }

        protected AnimationListener()
        {
        }

        public void Listener(string msg)
        {
            Current = this;
            if (msg != null && msg[0] == '{')
            {
                _parameter = JsonConvert.DeserializeObject<Dictionary<string, object>>(msg);
            }
            else
            {
                _parameter = msg;
            }

            m_OnEvent.Invoke();
        }
    }
}