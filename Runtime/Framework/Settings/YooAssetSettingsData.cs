using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using YooAsset;

namespace GhysX.Framework.Settings
{
    // [CreateAssetMenu(fileName = "GhysXYooAssetSettings", menuName = "GhysX/Create Settings For YooAsset")]
    // [InlineEditor]
    public class YooAssetSettingsData : ScriptableObject
    {
        
        [DisableInEditorMode]
        [LabelText("版本号")]
        public string version = "1.0.0";
        
        [LabelText("默认资源包名称")]
        public string defaultPackageName = "DefaultPackage";

        [LabelText("服务地址")]
        public string serverAddress = "http://127.0.0.1";
        
        [LabelText("服务备用地址")]
        public string alternateServerAddress = "http://127.0.0.1";
        
        [LabelText("运行模式")]
        [ValueDropdown(nameof(EPlayModeData))]
        public EPlayMode playMode = EPlayMode.HostPlayMode;

        [LabelText("目录")]
        public Object selectObject;
        
        private IEnumerable EPlayModeData => Attributes.YooAssetAttributeData.EPlayModeData;
        
    }
}
