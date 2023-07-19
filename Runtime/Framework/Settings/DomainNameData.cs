using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using YooAsset;

namespace GhysX.Framework.Settings
{
#if UNITY_EDITOR
    [CreateAssetMenu(fileName = "DomainNames", menuName = "GhysX/Create DomainNames")]
    [InlineEditor]
#endif
    public class DomainNameData : ScriptableObject
    {
        [TableList(ShowIndexLabels = true)] public List<DomainNameInfo> DomainNameInfos;
    }

    [Serializable]
    public struct DomainNameInfo
    {
        [TableColumnWidth(120, Resizable = false)]
        // [PreviewField(Alignment = ObjectFieldAlignment.Center)]
        // [LabelText("洲")]
        // [ValueDropdown(nameof(ContinentCode))]
        public ContinentCode continentCode;

        // [LabelText("URL")]
        public string url;

        // [LabelText("排序")]
        [TableColumnWidth(40, Resizable = false)]
        public int sort;
    }

    [Serializable]
    public enum ContinentCode
    {
        /// <summary>
        /// 非洲
        /// </summary>
        [LabelText("非洲")] AF,
        
        /// <summary>
        /// 欧洲
        /// </summary>
        [LabelText("欧洲")] EU,
        
        /// <summary>
        /// 亚洲
        /// </summary>
        [LabelText("亚洲")] AS,
        
        /// <summary>
        /// 大洋洲
        /// </summary>
        [LabelText("大洋洲")] OA,
        
        /// <summary>
        /// 北美洲
        /// </summary>
        [LabelText("北美洲")] NA,
        
        /// <summary>
        /// 南美洲
        /// </summary>
        [LabelText("南美洲")] SA,
        
        /// <summary>
        /// 南极洲
        /// </summary>
        [LabelText("南极洲")] AN,
    }
}