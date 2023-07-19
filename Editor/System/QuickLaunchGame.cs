using System;
using System.Collections.Generic;
using GhysX.Framework.Settings;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityToolbarExtender;

namespace GhysX.Framework.Editor
{
    [InitializeOnLoad]
    public static class QuickLaunchGame
    {
        private const string NeedRestoreSceneSaveKey = "Editor_NeedRestoreSceneAssetPath";
        private const string NeedRestorePrefabSaveKey = "Editor_NeedRestorePrefabAssetPath";

        private const string QuickLaunchNameSaveKey = "GhysX_Editor_QuickLaunchName";

        private static int _quickLaunchNameIndex = 0;

        static QuickLaunchGame()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            ToolbarExtender.LeftToolbarGUI.Add(OnToolBarGUILeft);
            ToolbarExtender.RightToolbarGUI.Add(OnToolBarGUIRight);

            _quickLaunchNameIndex = 0;

            var instanceQuickLaunchSettings = GhysXSettings.Instance.quickLaunchSettings as QuickLaunchData;
            if (instanceQuickLaunchSettings != null)
            {
                var quickLaunchNameName = EditorPrefs.GetString(QuickLaunchNameSaveKey);
                for (var i = 0; i < instanceQuickLaunchSettings.items.Count; i++)
                {
                    if (instanceQuickLaunchSettings.items[i].name == quickLaunchNameName)
                    {
                        _quickLaunchNameIndex = i;
                    }
                }
            }
        }

        private static void OnToolBarGUILeft()
        {
            if (EditorApplication.isPlaying)
            {
                return;
            }

            GUILayout.FlexibleSpace();

            EditorGUI.BeginChangeCheck();

            var entryGameFlowGraphNames = new List<string>();
            var instanceQuickLaunchSettings = GhysXSettings.Instance.quickLaunchSettings as QuickLaunchData;
            if (instanceQuickLaunchSettings != null)
            {
                for (var i = 0; i < instanceQuickLaunchSettings.items.Count; i++)
                {
                    var item = instanceQuickLaunchSettings.items[i];
                    entryGameFlowGraphNames.Add(item.name);
                }

                _quickLaunchNameIndex = EditorGUILayout.Popup(_quickLaunchNameIndex, entryGameFlowGraphNames.ToArray(),
                    GUILayout.Width(100));
            }

            if (EditorGUI.EndChangeCheck())
            {
                if (entryGameFlowGraphNames.Count > _quickLaunchNameIndex)
                {
                    PlayerPrefs.SetString(QuickLaunchNameSaveKey, entryGameFlowGraphNames[_quickLaunchNameIndex]);
                }
            }

            GUILayout.Space(20);
        }

        private static void OnToolBarGUIRight()
        {
            if (EditorApplication.isPlaying)
            {
                return;
            }

            if (GUILayout.Button(new GUIContent("启动游戏", "从当前场景快速启动游戏,停止游戏后恢复到当前场景"), GUILayout.MaxWidth(100)))
            {
                LaunchGame();
            }
        }

        private static void LaunchGame()
        {
            if (EditorApplication.isCompiling || EditorApplication.isPlaying || EditorApplication.isPaused)
            {
                return;
            }

            DeployAllScenes();

            // 记录当前打开的prefab
            var currentPrefabStage = PrefabStageUtility.GetCurrentPrefabStage();
            if (currentPrefabStage != null)
            {
                var prefabAssetPath = currentPrefabStage.assetPath;
                EditorPrefs.SetString(NeedRestorePrefabSaveKey, prefabAssetPath);
            }
            else
            {
                EditorPrefs.DeleteKey(NeedRestorePrefabSaveKey);
            }

            // 记录当前激活的场景
            var activeScene = SceneManager.GetActiveScene();
            if (activeScene.name == "GameFramework")
            {
                // 直接启动
                EditorApplication.isPlaying = true;
            }
            else
            {
                EditorPrefs.SetString(NeedRestoreSceneSaveKey, activeScene.path);
                string scenePath = "Assets/GameFramework.unity";
                EditorSceneManager.OpenScene(scenePath);
                EditorApplication.isPlaying = true;
            }
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredEditMode)
            {
                var needRestoreSceneAssetPath = EditorPrefs.GetString(NeedRestoreSceneSaveKey);
                if (!string.IsNullOrEmpty(needRestoreSceneAssetPath))
                {
                    try
                    {
                        EditorSceneManager.OpenScene(needRestoreSceneAssetPath);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                    }

                    EditorPrefs.DeleteKey(NeedRestoreSceneSaveKey);
                }

                var needRestorePrefabAssetPath = EditorPrefs.GetString(NeedRestorePrefabSaveKey);
                if (!string.IsNullOrEmpty(needRestorePrefabAssetPath))
                {
                    var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(needRestorePrefabAssetPath);
                    try
                    {
                        AssetDatabase.OpenAsset(prefab);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                    }

                    EditorPrefs.DeleteKey(needRestorePrefabAssetPath);
                }
            }
            else if (state == PlayModeStateChange.EnteredPlayMode)
            {
                // stayAtSceneViewWhenPlayGame = PlayerPrefs.GetInt(StayAtSceneViewSaveKey) == 1;
                // if (stayAtSceneViewWhenPlayGame)
                // {
                //     EditorWindow.FocusWindowIfItsOpen(typeof(SceneView));
                //     Debug.Log("focus scene view");
                // }
            }
        }

        private static void DeployAllScenes()
        {
            var sceneNames = new HashSet<string> { "Assets/GameFramework.unity" };
            var sceneGuids = AssetDatabase.FindAssets("t:Scene", new[] { "Assets/Game/Scene" });
            foreach (var sceneGuid in sceneGuids)
            {
                var sceneName = AssetDatabase.GUIDToAssetPath(sceneGuid);
                sceneNames.Add(sceneName);
            }

            var scenes = new List<EditorBuildSettingsScene>();
            foreach (var sceneName in sceneNames)
            {
                scenes.Add(new EditorBuildSettingsScene(sceneName, true));
            }

            EditorBuildSettings.scenes = scenes.ToArray();
        }
    }
}