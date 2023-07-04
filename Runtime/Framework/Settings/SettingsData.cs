using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using YooAsset;

namespace GhysX.Framework.Settings
{
    [CreateAssetMenu(fileName = "GhysXSettings", menuName = "GhysX/Create Settings")]
    [InlineEditor]
    public class SettingsData : ScriptableObject
    {
        [TabGroup("General")]
        [DisableInEditorMode]
        public string version = "1.0.0";

        [TabGroup("Server")]
        public string serverAddress = "http://127.0.0.1";
        
        [TabGroup("Runtime")]
        public EPlayMode playMode = EPlayMode.HostPlayMode;
    }
}
