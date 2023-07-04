using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using GhysX.Framework.Settings;
using Sirenix.OdinInspector;

namespace GhysX.Framework.Editor
{
    public class SettingsEditor : OdinMenuEditorWindow
    {
        [MenuItem("Tools/GhysX/Settings")]
        private static void Open()
        {
            var window = GetWindow<SettingsEditor>("GhysX Settings");
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 500);
        }
        
        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree(true);
            tree.DefaultMenuStyle.IconSize = 28.00f;
            tree.Config.DrawSearchToolbar = true;
            
            // tree.Add("General", new GeneralSetting());
            tree.Add("General", GhysXSettings.Instance.settings as SettingsData);
            tree.Add("Yoo Asset", GhysXSettings.Instance.settings as SettingsData);

            return tree;
        }
    }
    
    public class GeneralSetting
    {
        [AssetsOnly]
        public ScriptableObject settingsData;

        public GeneralSetting()
        {
            settingsData = GhysXSettings.Instance.settings as SettingsData;
        }
    }
}
