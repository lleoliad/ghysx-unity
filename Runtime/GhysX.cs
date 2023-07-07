using System.Collections;
using System.Globalization;
using GhysX.Framework.Extensions;
using GhysX.Framework.Settings;
using GhysX.Framework.YooAsset;
using Loxodon.Framework.Asynchronous;
using Loxodon.Framework.Binding;
using Loxodon.Framework.Contexts;
using Loxodon.Framework.Localizations;
using Loxodon.Framework.Messaging;
using Loxodon.Framework.Services;
using Loxodon.Framework.Views;
using UniFramework.Event;
using UniFramework.Singleton;
using UnityEngine;
using YooAsset;
using YooAssets = YooAsset.YooAssets;

namespace GhysX.Framework
{
    public static partial class GhysX
    {
        private static bool _isInitialize = false;
        private static bool _isLoxodonFrameworkInitialize = false;
        private static GameObject _driver = null;
        public static SettingsData Settings { get; set; }

        public static YooAssetSettingsData YooAssetSettings { get; set; }
        
        private static IMessenger Messenger { get; set; }

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

                InitializeUniFramework();
            }
        }

        public static void InitializeUniFramework()
        {
            UnityEngine.Debug.LogWarning($"{nameof(GhysX)} & UniFramework Initialize !");
            
            // 初始化事件系统
            UniEvent.Initalize();

            // 初始化单例系统
            UniSingleton.Initialize();
        }

        public static void InitializeYooAsset()
        {
            UnityEngine.Debug.LogWarning($"{nameof(GhysX)} & YooAsset Initialize !");
            
            YooAssetSettings = UnityEngine.Resources.Load<YooAssetSettingsData>("GhysXYooAssetSettings");

            UnityEngine.Debug.Log($"GhysX - YooAsset Settings: {JsonUtility.ToJson(YooAssetSettings)}");
            
            // 初始化资源系统
            YooAssets.Initialize();
            
            YooAssets.SetOperationSystemMaxTimeSlice(30);

            AsyncTask task = new AsyncTask(InitializeYooAssetPackage(), true);

            /* Start the task */
            task.OnPreExecute(() =>
            {
                Debug.Log("The task has started.");
            }).OnPostExecute(() =>
            {
                Debug.Log("The task has completed.");/* only execute successfully */
            }).OnError((e) =>
            {
                Debug.LogFormat("An error occurred:{0}", e);
            }).OnFinish(() =>
            {
                Debug.Log("The task has been finished.");/* completed or error or canceled*/
            }).Start();
        }

        public static IEnumerator InitializeYooAssetPackage()
        {
            UnityEngine.Debug.LogWarning($"{nameof(GhysX)} & YooAsset Package Initialize !");
            
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
                createParameters.DefaultHostServer = YooAsset.YooAssets.GetHostServerURL();
                createParameters.FallbackHostServer = YooAsset.YooAssets.GetHostServerURL();
                initializationOperation = package.InitializeAsync(createParameters);
            }

            yield return initializationOperation;
            
            if (initializationOperation.Status == EOperationStatus.Succeed)
            {
                // _machine.ChangeState<FsmUpdateVersion>();
                UnityEngine.Debug.LogWarning($"{nameof(GhysX)} & YooAsset Package Initialize Succeed !!!");

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

                // var handle = package.LoadAssetAsync<GameObject>("Logo");
                // yield return handle;
                // GameObject logo = handle.InstantiateSync();
                
                GlobalWindowManager.Root.transform.DestroyChildrenImmediate();
                IUIViewLocator locator = Context.GetApplicationContext().GetService<IUIViewLocator>();
                var window = locator.LoadWindow("Logo");
                window.Create();
                ITransition transition = window.Show().OnStateChanged((w, state) =>
                {
                    // log.DebugFormat("Window:{0} State{1}",w.Name,state);
                });

                yield return transition.WaitForDone();
            }
            else
            {
                Debug.LogWarning($"{initializationOperation.Error}");
                // PatchEventDefine.InitializeFailed.SendEventMessage();
                // UnityEngine.Debug.LogWarning($"{nameof(GhysX)} & YooAsset Package Initialize Fail !!!");
                Messenger.Publish(new InitializePackageErrorMessage(_driver));
            }
        }

        public static void InitializeLoxodonFramework()
        {
            if (_isLoxodonFrameworkInitialize == false)
            {
                UnityEngine.Debug.LogWarning($"{nameof(GhysX)} & Loxodon.Framework Initialize !");
                _isLoxodonFrameworkInitialize = true;
                GlobalWindowManager windowManager = UnityEngine.Object.FindObjectOfType<GlobalWindowManager>();
                if (windowManager == null)
                    throw new NotFoundException("Not found the GlobalWindowManager.");

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

                /* register AccountRepository */
                // IAccountRepository accountRepository = new AccountRepository();
                // container.Register<IAccountService>(new AccountService(accountRepository));

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