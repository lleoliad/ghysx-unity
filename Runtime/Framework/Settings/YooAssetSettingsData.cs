using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using YooAsset;

namespace GhysX.Framework.Settings
{
#if UNITY_EDITOR
    [CreateAssetMenu(fileName = "GhysXYooAssetSettings", menuName = "GhysX/Create Settings For YooAsset")]
    [InlineEditor]
#endif
    public class YooAssetSettingsData : ScriptableObject
    {
        [DisableInEditorMode] [LabelText("版本号")]
        public string version = "1.0.0";

        [LabelText("默认资源包名称")] public string defaultPackageName = "DefaultPackage";

        [LabelText("资源版本号")] public string resourceVersion = "1.0.0";

        [LabelText("服务地址")] public string defaultHostServer = "http://127.0.0.1/";

        [LabelText("备用主机服务器")] public string fallbackHostServer = "http://127.0.0.1";

        [LabelText("运行模式")] [ValueDropdown(nameof(EPlayModeData))]
        public EPlayMode playMode = EPlayMode.HostPlayMode;

        [LabelText("目录")] public Object selectObject;

        private IEnumerable EPlayModeData => Attributes.YooAssetAttributeData.EPlayModeData;
    }
}