using System;
using System.Collections.Generic;
using System.IO;
using Loxodon.Framework.Messaging;
using UnityEngine;

namespace YooAsset
{
    public static class YooAssetsHelper
    {
        /// <summary>
        /// 获取资源服务器地址
        /// </summary>
        public static string GetHostServerURL(string host, string version)
        {
            string hostServerIP = host;
            string appVersion = version;
#if UNITY_EDITOR
            if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.Android)
                return $"{hostServerIP}/Android/{appVersion}";
            else if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.iOS)
                return $"{hostServerIP}/iOS/{appVersion}";
            else if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.WebGL)
                return $"{hostServerIP}/WebGL/{appVersion}";
            else if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.StandaloneOSX)
                return $"{hostServerIP}/OSX/{appVersion}";
            else
                return $"{hostServerIP}/Windows/{appVersion}";
#else
		if (Application.platform == RuntimePlatform.Android)
			return $"{hostServerIP}/Android/{appVersion}";
		else if (Application.platform == RuntimePlatform.IPhonePlayer)
			return $"{hostServerIP}/iOS/{appVersion}";
		else if (Application.platform == RuntimePlatform.WebGLPlayer)
			return $"{hostServerIP}/WebGL/{appVersion}";
        else if (Application.platform == RuntimePlatform.OSXPlayer)
			return $"{hostServerIP}/OSX/{appVersion}";
		else
			return $"{hostServerIP}/Windows/{appVersion}";
#endif
        }
    }

    public class BundleStream : FileStream
    {
        public const byte KEY = 64;

        public BundleStream(string path, FileMode mode, FileAccess access, FileShare share) : base(path, mode, access,
            share)
        {
        }

        public BundleStream(string path, FileMode mode) : base(path, mode)
        {
        }

        public override int Read(byte[] array, int offset, int count)
        {
            var index = base.Read(array, offset, count);
            for (int i = 0; i < array.Length; i++)
            {
                array[i] ^= KEY;
            }

            return index;
        }
    }

    /// <summary>
    /// 资源文件解密服务类
    /// </summary>
    public class GameDecryptionServices : IDecryptionServices
    {
        public ulong LoadFromFileOffset(DecryptFileInfo fileInfo)
        {
            return 32;
        }

        public byte[] LoadFromMemory(DecryptFileInfo fileInfo)
        {
            throw new NotImplementedException();
        }

        public Stream LoadFromStream(DecryptFileInfo fileInfo)
        {
            BundleStream bundleStream =
                new BundleStream(fileInfo.FilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            return bundleStream;
        }

        public uint GetManagedReadBufferSize()
        {
            return 1024;
        }
    }

    /// <summary>
    /// 内置文件查询服务类
    /// </summary>
    public class GameQueryServices : IQueryServices
    {
        public bool QueryStreamingAssets(string fileName)
        {
            // 注意：fileName包含文件格式
            return StreamingAssetsHelper.FileExists(fileName);
        }
    }

#if UNITY_EDITOR
    /// <summary>
    /// StreamingAssets目录下资源查询帮助类
    /// </summary>
    public sealed class StreamingAssetsHelper
    {
        public static void Init()
        {
        }

        public static bool FileExists(string fileName)
        {
            return File.Exists(Path.Combine(Application.streamingAssetsPath, "BuildinFiles", fileName));
        }
    }
#else
    /// <summary>
    /// StreamingAssets目录下资源查询帮助类
    /// </summary>
    public sealed class StreamingAssetsHelper
    {
        private static bool _isInit = false;
        private static readonly HashSet<string> _cacheData = new HashSet<string>();

        /// <summary>
        /// 初始化
        /// </summary>
        public static void Init()
        {
            if (_isInit == false)
            {
                _isInit = true;
                var manifest = Resources.Load<BuildinFileManifest>("BuildinFileManifest");
                foreach (string fileName in manifest.BuildinFiles)
                {
                    _cacheData.Add(fileName);
                }
            }
        }

        /// <summary>
        /// 内置文件查询方法
        /// </summary>
        public static bool FileExists(string fileName)
        {
            if (_isInit == false)
                Init();
            return _cacheData.Contains(fileName);
        }
    }
#endif
    
    /// <summary>
    /// 内置资源清单
    /// </summary>
    public class BuildinFileManifest : ScriptableObject
    {
        public List<string> BuildinFiles = new List<string>();
    }

    public class InitializePackageSuccessMessage : MessageBase
    {
        public InitializePackageSuccessMessage(object sender) : base(sender)
        {
        }
    }

    public class InitializePackageErrorMessage : MessageBase
    {
        public InitializePackageErrorMessage(object sender) : base(sender)
        {
        }
    }
}