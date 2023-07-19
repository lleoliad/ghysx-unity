using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GhysX.Framework.Settings
{
#if UNITY_EDITOR
    [CreateAssetMenu(fileName = "QuickLaunchSettings", menuName = "GhysX/Create Quick Launch")]
    [InlineEditor]
#endif
    public class QuickLaunchData : ScriptableObject
    {
        [TableList(ShowIndexLabels = true)] public List<SceneInfo> items;
    }

    [Serializable]
    public struct SceneInfo
    {
        public Object selectObject;
        
        public string name;
    }
}