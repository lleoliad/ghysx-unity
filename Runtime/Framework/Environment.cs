using System;
using System.Collections;
using System.Globalization;
using GhysX.Framework.External.Prefs;
using GhysX.Framework.Repositories;
using GhysX.Framework.Services;
using GhysX.Framework.Settings;
using Loxodon.Framework.Asynchronous;
using Loxodon.Framework.Binding;
using Loxodon.Framework.Contexts;
using Loxodon.Framework.Localizations;
using Loxodon.Framework.Messaging;
using Loxodon.Framework.Prefs;
using Loxodon.Framework.Services;
using Loxodon.Framework.Views;
using UniFramework.Event;
using UniFramework.Singleton;
using UnityEngine;
using YooAsset;
using YooAssets = YooAsset.YooAssets;

namespace GhysX.Framework
{
    public static partial class Environment
    {
        private static bool _isInitialize = false;
        private static bool _isLoxodonFrameworkInitialize = false;
        private static GameObject _driver = null;
        public static SettingsData Settings { get; set; }

        public static DomainNameData DomainNames { get; set; }

        public static YooAssetSettingsData YooAssetSettings { get; set; }

        public static IMessenger Messenger { get; set; }

        public static WindowManager WindowManager { get; set; }

        public static void Initialize()
        {
            if (_isInitialize)
            {
                UnityEngine.Debug.LogWarning($"{nameof(Environment)} is initialized !");
                return;
            }

            if (_isInitialize == false)
            {
                _isInitialize = true;
                _driver = new UnityEngine.GameObject($"[{nameof(Environment)}]");
                UnityEngine.Object.DontDestroyOnLoad(_driver);
                UnityEngine.Debug.Log($"{nameof(Environment)} initialize !");

                Settings = UnityEngine.Resources.Load<SettingsData>("GhysXSettings");
                UnityEngine.Debug.Log($"GhysX Settings: {JsonUtility.ToJson(Settings)}");
                
                DomainNames = UnityEngine.Resources.Load<DomainNameData>("DomainNames");
                UnityEngine.Debug.Log($"GhysX DomainNames: {JsonUtility.ToJson(DomainNames)}");

                InitializeUniFramework();
            }
        }

        public static void InitializeUniFramework()
        {
            UnityEngine.Debug.LogWarning($"{nameof(Environment)} & UniFramework Initialize !");

            // 初始化事件系统
            UniEvent.Initalize();

            // 初始化单例系统
            UniSingleton.Initialize();
        }

        public static void InitializeYooAsset()
        {
            UnityEngine.Debug.LogWarning($"{nameof(Environment)} & YooAsset Initialize !");

            YooAssetSettings = UnityEngine.Resources.Load<YooAssetSettingsData>("GhysXYooAssetSettings");

            UnityEngine.Debug.Log($"GhysX - YooAsset Settings: {JsonUtility.ToJson(YooAssetSettings)}");

            // 初始化资源系统
            YooAssets.Initialize();

            YooAssets.SetOperationSystemMaxTimeSlice(30);

            AsyncTask task = new AsyncTask(InitializeYooAssetPackage(), true);

            /* Start the task */
            task.OnPreExecute(() => { Debug.Log("The task has started."); })
                .OnPostExecute(() => { Debug.Log("The task has completed."); /* only execute successfully */ })
                .OnError((e) => { Debug.LogFormat("An error occurred:{0}", e); }).OnFinish(() =>
                {
                    Debug.Log("The task has been finished."); /* completed or error or canceled*/
                }).Start();
        }

        public static IEnumerator InitializeYooAssetPackage()
        {
            UnityEngine.Debug.LogWarning($"{nameof(Environment)} & YooAsset Package Initialize !");

            var packageName = YooAssetSettings.defaultPackageName;

            // 创建默认的资源包
            var package = YooAssets.CreatePackage(YooAssetSettings.defaultPackageName);

            // 设置该资源包为默认的资源包，可以使用YooAssets相关加载接口加载该资源包内容。  
            YooAssets.SetDefaultPackage(package);

            var playMode = YooAssetSettings.playMode;

            InitializationOperation initializationOperation = null;

            // 编辑器下的模拟模式
            if (playMode == EPlayMode.EditorSimulateMode)
            {
                var createParameters = new EditorSimulateModeParameters();
                createParameters.SimulateManifestFilePath = EditorSimulateModeHelper.SimulateBuild(packageName);
                initializationOperation = package.InitializeAsync(createParameters);
            }

            // 单机运行模式
            if (playMode == EPlayMode.OfflinePlayMode)
            {
                var createParameters = new OfflinePlayModeParameters();
                createParameters.DecryptionServices = new GameDecryptionServices();
                initializationOperation = package.InitializeAsync(createParameters);
            }

            // 联机运行模式
            if (playMode == EPlayMode.HostPlayMode)
            {
                var createParameters = new HostPlayModeParameters();
                createParameters.DecryptionServices = new GameDecryptionServices();
                createParameters.QueryServices = new GameQueryServices();
                createParameters.DefaultHostServer =
                    YooAssetsHelper.GetHostServerURL(YooAssetSettings.defaultHostServer, YooAssetSettings.version);
                createParameters.FallbackHostServer =
                    YooAssetsHelper.GetHostServerURL(YooAssetSettings.fallbackHostServer, YooAssetSettings.version);
                initializationOperation = package.InitializeAsync(createParameters);
            }

            yield return initializationOperation;

            if (initializationOperation.Status == EOperationStatus.Succeed)
            {
                // _machine.ChangeState<FsmUpdateVersion>();
                UnityEngine.Debug.LogWarning($"{nameof(Environment)} & YooAsset Package Initialize Succeed !!!");

                // 重新读取配置
                if (package.CheckLocationValid("GhysXYooAssetSettings"))
                {
                    var assetOperationHandle = package.LoadAssetSync<YooAssetSettingsData>("GhysXYooAssetSettings");
                    if (assetOperationHandle.Status == EOperationStatus.Succeed)
                    {
                        YooAssetSettings = assetOperationHandle.GetAssetObject<YooAssetSettingsData>();
                    }
                }

                Messenger.Publish(new InitializePackageSuccessMessage(_driver));

                // // var handle = package.LoadAssetAsync<GameObject>("Logo");
                // // yield return handle;
                // // GameObject logo = handle.InstantiateSync();
                //
                // GlobalWindowManager.Root.transform.DestroyChildrenImmediate();
                // IUIViewLocator locator = Context.GetApplicationContext().GetService<IUIViewLocator>();
                // var window = locator.LoadWindow("Logo");
                // window.Create();
                // ITransition transition = window.Show().OnStateChanged((w, state) =>
                // {
                //     // log.DebugFormat("Window:{0} State{1}",w.Name,state);
                // });
                //
                // yield return transition.WaitForDone();
            }
            else
            {
                Debug.LogWarning($"{initializationOperation.Error}");
                // PatchEventDefine.InitializeFailed.SendEventMessage();
                // UnityEngine.Debug.LogWarning($"{nameof(GhysX)} & YooAsset Package Initialize Fail !!!");
                Messenger.Publish(new InitializePackageErrorMessage(_driver));
            }
        }

        #region Initialize Loxodon.Framework

        public static void InitializeLoxodonFramework()
        {
            if (_isLoxodonFrameworkInitialize == false)
            {
                UnityEngine.Debug.LogWarning($"{nameof(Environment)} & Loxodon.Framework Initialize !");
                _isLoxodonFrameworkInitialize = true;
                GlobalWindowManager windowManager = UnityEngine.Object.FindObjectOfType<GlobalWindowManager>();
                if (windowManager == null)
                    throw new NotFoundException("Not found the GlobalWindowManager.");

                WindowManager = windowManager;

                ApplicationContext context = Context.GetApplicationContext();

                IServiceContainer container = context.GetContainer();

                /* Initialize the data binding service */
                BindingServiceBundle bundle = new BindingServiceBundle(context.GetContainer());
                bundle.Start();

                /* Initialize the ui view locator and register UIViewLocator */
                // container.Register<IUIViewLocator>(new DefaultUIViewLocator());
                container.Register<IUIViewLocator>(new YooAesstUIViewLocator());

                /* Initialize the localization service */
                CultureInfo cultureInfo = Locale.GetCultureInfo();
                var localization = Localization.Current;
                localization.CultureInfo = cultureInfo;
                localization.AddDataProvider(new DefaultDataProvider("LocalizationExamples", new XmlDocumentParser()));

                /* register Localization */
                container.Register<Localization>(localization);

                /* register AuthRepository */
                IAuthRepository authRepository = new AuthRepository();
                container.Register<IAuthService>(new AuthService(authRepository));

                /* Enable window state broadcast */
                GlobalSetting.enableWindowStateBroadcast = true;
                /* 
                 * Use the CanvasGroup.blocksRaycasts instead of the CanvasGroup.interactable 
                 * to control the interactivity of the view
                 */
                GlobalSetting.useBlocksRaycastsInsteadOfInteractable = true;

                // /* Subscribe to window state change events */
                // subscription = Window.Messenger.Subscribe<WindowStateEventArgs>(e =>
                // {
                //     Debug.LogFormat("The window[{0}] state changed from {1} to {2}", e.Window.Name, e.OldState, e.State);
                // });

                Messenger = new Messenger();

                // Initialize Prefs
                InitializeLoxodonFrameworkPreferences();
            }
        }

        public static void InitializeLoxodonFrameworkPreferences()
        {
            BinaryFilePreferencesFactory factory = new BinaryFilePreferencesFactory();

            /* Custom a ITypeEncoder for the type of CustomData. */
            factory.Serializer.AddTypeEncoder(new CustomDataTypeEncoder<CustomData>());

            Preferences.Register(factory);

            // /* This is a global preferences. */
            // Preferences prefs = Preferences.GetGlobalPreferences();
            // prefs.SetString("username", "clark_ya@163.com");
            // prefs.SetString("name", "clark");
            // prefs.SetInt("zone", 5);
            // prefs.Save();
            //
            // /* This is a preferences that it's only clark's data in the fifth zone. */
            // Preferences userPrefs = Preferences.GetPreferences("clark@5"); /* username:clark, zone:5 */
            // userPrefs.SetString("role.name", "clark");
            // userPrefs.SetObject("role.logout.map.position", new Vector3(1f, 2f, 3f));
            // userPrefs.SetObject("role.logout.map.forward", new Vector3(0f, 0f, 1f));
            // userPrefs.SetObject("role.logout.time", DateTime.Now);
            // userPrefs.SetObject("test.custom.data", new CustomData("test", "This is a test."));
            // userPrefs.Save();

            // Debug.LogFormat("username:{0}; name:{1}; zone:{2};", prefs.GetString("username"), prefs.GetString("name"), prefs.GetInt("zone"));
            // Debug.LogFormat("position:{0} forward:{1} logout time:{2}", userPrefs.GetObject<Vector3>("role.logout.map.position"), userPrefs.GetObject<Vector3>("role.logout.map.forward"), userPrefs.GetObject<DateTime>("role.logout.time"));
            // Debug.LogFormat("CustomData name:{0}   description:{1}", userPrefs.GetObject<CustomData>("test.custom.data").name, userPrefs.GetObject<CustomData>("test.custom.data").description);
        }

        #endregion

        public static void Destroy()
        {
            if (_isInitialize)
            {
                _isInitialize = false;
                if (_driver != null)
                    GameObject.Destroy(_driver);
                UnityEngine.Debug.Log($"{nameof(Environment)} destroy all !");
            }
        }

        [RuntimeInitializeOnLoadMethod]
        static void RuntimeOnLoadMethod()
        {
            // 场景加载完毕后执行（缺省值）
            // UnityEngine.Debug.Log("Step 6 : RuntimeOnLoadMethod");
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void AfterSceneRuntimeOnLoadMethod()
        {
            // 场景加载完毕后执行
            // UnityEngine.Debug.Log("Step 5 : AfterSceneRuntimeOnLoadMethod");
            GlobalWindowManager windowManager = UnityEngine.Object.FindObjectOfType<GlobalWindowManager>();
            if (windowManager == null)
            {
                var gameObject = UnityEngine.Resources.Load<GameObject>("UI/Launcher/Launcher");
                GameObject.Instantiate(gameObject);
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void BeforeSceneRuntimeOnLoadMethod()
        {
            // 场景加载前执行
            // UnityEngine.Debug.Log("Step 4 : BeforeSceneRuntimeOnLoadMethod");
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        static void AfterAssembliesLoadedRuntimeOnLoadMethod()
        {
            // 所有程序集被加载和预加载资产被初始化后调用
            // UnityEngine.Debug.Log("Step 2 : AfterAssembliesLoadedRuntimeOnLoadMethod");
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        static void BeforeSplashScreenRuntimeOnLoadMethod()
        {
            // 启动画面显示之前调用
            // UnityEngine.Debug.Log("Step 3 : BeforeSplashScreenRuntimeOnLoadMethod");
            Initialize();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void SubsystemRegistrationRuntimeOnLoadMethod()
        {
            // 子系统注册回调
            // UnityEngine.Debug.Log("Step 1 : SubsystemRegistrationRuntimeOnLoadMethod");
        }
    }
}