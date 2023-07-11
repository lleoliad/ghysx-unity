using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using YooAsset;

namespace GhysX.Framework.Settings
{
    // [CreateAssetMenu(fileName = "GhysXSettings", menuName = "GhysX/Create Settings")]
    // [InlineEditor]
    public class SettingsData : ScriptableObject
    {
        
        [TabGroup("General")]
        [DisableInEditorMode]
        [LabelText("版本号")]
        public string version = "1.0.0";
        
        // [TabGroup("General")]
        // [LabelText("YooAsset")]
        // public bool yooAssetEnabled = true;

        // [TabGroup("YooAsset")]
        // [LabelText("默认资源包名称")]
        // public string defaultPackageName = "DefaultPackage";
        
        [FormerlySerializedAs("AuthServerAddress")]
        [TabGroup("Server")]
        [LabelText("授权服务地址")]
        public string authServerAddress = "http://127.0.0.1";

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

        [TabGroup("Runtime")] 
        [LabelText("目录")]
        public Object selectObject;
        
        private IEnumerable EPlayModeData => Attributes.YooAssetAttributeData.EPlayModeData;
        
    }
}
