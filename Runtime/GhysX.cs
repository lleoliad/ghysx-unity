using System.Collections;
using System.Collections.Generic;
using GhysX.Framework.Settings;
using UniFramework.Event;
using UniFramework.Singleton;
using UnityEngine;
using YooAsset;

namespace GhysX.Framework
{
    public static partial class GhysX
    {
        private static bool _isInitialize = false;
        private static GameObject _driver = null;
        public static SettingsData Settings { get; set; }

        public static void Initialize()
        {
            if (_isInitialize)
            {
                UnityEngine.Debug.LogWarning($"{nameof(GhysX)} is initialized !");
                return;
            }

            if (_isInitialize == false)
            {
                _isInitialize = true;
                _driver = new UnityEngine.GameObject($"[{nameof(GhysX)}]");
                UnityEngine.Object.DontDestroyOnLoad(_driver);
                UnityEngine.Debug.Log($"{nameof(GhysX)} initialize !");

                Settings = UnityEngine.Resources.Load<SettingsData>("GhysXSettings");
                UnityEngine.Debug.Log($"GhysX Settings: {JsonUtility.ToJson(Settings)}");
                
                // 初始化事件系统
                UniEvent.Initalize();

                // 初始化单例系统
                UniSingleton.Initialize();
                
                // 初始化资源系统
                YooAssets.Initialize();
                YooAssets.SetOperationSystemMaxTimeSlice(30);
            }
        }

        public static void Destroy()
        {
            if (_isInitialize)
            {
                _isInitialize = false;
                if (_driver != null)
                    GameObject.Destroy(_driver);
                UnityEngine.Debug.Log($"{nameof(GhysX)} destroy all !");
            }
        }

        [RuntimeInitializeOnLoadMethod]
        static void RuntimeOnLoadMethod()
        {
            // 场景加载完毕后执行（缺省值）
            // UnityEngine.Debug.Log("RuntimeOnLoadMethod");
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void AfterSceneRuntimeOnLoadMethod()
        {
            // 场景加载完毕后执行
            // UnityEngine.Debug.Log("AfterSceneRuntimeOnLoadMethod");
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void BeforeSceneRuntimeOnLoadMethod()
        {
            // 场景加载前执行
            // UnityEngine.Debug.Log("BeforeSceneRuntimeOnLoadMethod");
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        static void AfterAssembliesLoadedRuntimeOnLoadMethod()
        {
            // 所有程序集被加载和预加载资产被初始化后调用
            // UnityEngine.Debug.Log("AfterAssembliesLoadedRuntimeOnLoadMethod");
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        static void BeforeSplashScreenRuntimeOnLoadMethod()
        {
            // 启动画面显示之前调用
            // UnityEngine.Debug.Log("BeforeSplashScreenRuntimeOnLoadMethod");
            Initialize();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void SubsystemRegistrationRuntimeOnLoadMethod()
        {
            // 子系统注册回调
            // UnityEngine.Debug.Log("SubsystemRegistrationRuntimeOnLoadMethod");
        }
    }
}