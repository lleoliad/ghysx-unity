using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
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
        [LabelText("版本号")]
        public string version = "1.0.0";
        
        [TabGroup("General")]
        [LabelText("YooAsset")]
        public bool yooAssetEnabled = true;

        [TabGroup("Server")]
        [LabelText("服务地址")]
        public string serverAddress = "http://127.0.0.1";
        
        [TabGroup("Server")]
        [LabelText("服务备用地址")]
        public string alternateServerAddress = "http://127.0.0.1";
        
        [TabGroup("Runtime")]
        [LabelText("运行模式")]
        [ValueDropdown(nameof(EPlayModeData))]
        public EPlayMode playMode = EPlayMode.HostPlayMode;
        
        private IEnumerable EPlayModeData => Attributes.YooAssetAttributeData.EPlayModeData;
        
    }
}
