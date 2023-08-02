using GhysX.Framework.Extensions;
using Loxodon.Framework.Contexts;
using Loxodon.Framework.Messaging;
using Loxodon.Framework.Views;
using UnityEngine;
using YooAsset;

namespace GhysX.Framework
{
    internal class Launcher : MonoBehaviour
    {
        private ISubscription<InitializePackageSuccessMessage> _subscription;

        void Awake()
        {
            gameObject.name = $"[GhysX.{nameof(Launcher)}]";
            DontDestroyOnLoad(gameObject);

            Environment.InitializeLoxodonFramework();
            Environment.InitializeYooAsset();
        }

        void Start()
        {
            this._subscription = Environment.Messenger.Subscribe<InitializePackageSuccessMessage>(OnMessage);
        }

        private void OnMessage(InitializePackageSuccessMessage message)
        {
            Environment.WindowManager.transform.DestroyChildrenImmediate(); // clear's background object.
            IUIViewLocator locator = Context.GetApplicationContext().GetService<IUIViewLocator>();
            var window = locator.LoadWindow<UIWindow>("Logo");
            window.Create();
            ITransition transition = window.Show().OnStateChanged((w, state) =>
            {
                Debug.LogFormat("Window:{0} State {1}", w.Name, state);
            });
        }

        void OnDestroy()
        {
            if (this._subscription != null)
            {
                this._subscription.Dispose();
                this._subscription = null;
            }
        }
    }
}