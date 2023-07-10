using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace GhysX.Framework.Anims
{
    public class AnimationListener : MonoBehaviour
    {
        [Serializable]
        public class AnimationEvent : UnityEvent {}
        
        [FormerlySerializedAs("onEvent")]
        [SerializeField]
        private AnimationEvent m_OnEvent = new AnimationEvent();
        
        public AnimationEvent onEvent
        {
            get { return m_OnEvent; }
            set { m_OnEvent = value; }
        }
        
        protected AnimationListener()
        {}
        
        public void Listener(string msg)
        {
            m_OnEvent.Invoke();
        }
    }
}