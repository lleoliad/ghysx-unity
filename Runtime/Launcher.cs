using System;
using System.Collections;
using System.Collections.Generic;
using GhysX.Framework.Settings;
using UnityEngine;
using YooAsset;

namespace GhysX.Framework
{
    internal class Launcher : MonoBehaviour
    {
        void Awake()
        {
            string appName = Application.productName;
            UnityEngine.Debug.Log($"GhysX:Launcher: {GhysX.Settings.playMode}");
            // UnityEngine.Debug.Log($"GhysX:Launcher: {appName}");
            GhysX.InitializeLoxodonFramework();
            GhysX.InitializeYooAsset();
        }

        void Start()
        {
            gameObject.name = $"GhysX.[{nameof(Launcher)}]";
            UnityEngine.Object.DontDestroyOnLoad(gameObject);
            UnityEngine.Debug.Log($"GhysX:Launcher1: {this.gameObject.name}");
        }
    }
}