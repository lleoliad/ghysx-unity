using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using YooAsset;

namespace GhysX.Framework.Attributes
{
    public class YooAssetAttribute : OdinSerializeAttribute
    {
        
    }
    
    public static class YooAssetAttributeData
    {
        public static readonly IEnumerable EPlayModeData = new ValueDropdownList<EPlayMode>()
        {
            { "编辑器下的模拟模式", EPlayMode.EditorSimulateMode },
            { "离线运行模式", EPlayMode.OfflinePlayMode },
            { "联机运行模式", EPlayMode.HostPlayMode },
        };
    }
}
