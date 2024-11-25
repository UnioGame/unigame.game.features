namespace Game.GameSettings
{
    using System;
    using Newtonsoft.Json;
    using Runtime;
    using Sirenix.OdinInspector;
    using UniModules;
    using UnityEngine;
    using Object = UnityEngine.Object;

#if UNITY_EDITOR
    using UnityEditor;
#endif
    
    /// <summary>
    /// Game Settings Service Source
    /// </summary>
    [CreateAssetMenu(menuName = "Game/Services/GameSettings/Remote Settings Asset", 
        fileName = "Remote Settings Asset")]
    public class RemoteModelAsset : ScriptableObject
    {
        #region static data
        
        private static string _remoteDefaultDataUrl = string.Empty;
        private static string _remoteStreamingAssetDataUrl = string.Empty;
        private static string _buildTargetUrl = string.Empty;
        private static string _remoteVersionUrl = string.Empty;
        private static string _remoteAppUrl = string.Empty;

        public const string RemoteSettingsName = "RemoteSettings.json";
        public const string EmptyPath = "[EMPTY_REMOTE_ASSET]";

        [Button]
        public void ResetRemote()
        {
            Reset();
        }

        [Button]
        public void RefreshPreview()
        {
            _demoUrl = GetAddressableRemoteUrl(this.data);
        }
        
        public static RemoteModelAsset modelAsset;

#if UNITY_EDITOR
        [InitializeOnLoadMethod]
        [MenuItem(itemName:"Game/Reset Remote Settings")]
#endif
        public static void Reset()
        {
            _remoteDefaultDataUrl = string.Empty;
            _remoteStreamingAssetDataUrl = string.Empty;
            _buildTargetUrl = string.Empty;
            _remoteVersionUrl = string.Empty;
            _remoteAppUrl = string.Empty;
        }

        public static string BundleUrl
        {
            get
            {
                if (!string.IsNullOrEmpty(_remoteDefaultDataUrl))
                    return _remoteDefaultDataUrl;
                
                var asset = ModelAsset;
                if (asset == null) return EmptyPath;
                
                var url = GetAddressableRemoteUrl(asset.data);
                
                _remoteDefaultDataUrl = url;
                return _remoteDefaultDataUrl;
            }
        }

        public static string AppVersion => Application.version;
        
        public static RemoteModelAsset ModelAsset
        {
            get
            {
                if(modelAsset != null) return modelAsset;
                modelAsset = Resources.Load<RemoteModelAsset>(nameof(RemoteModelAsset));
                
                if(modelAsset == null)
                    Debug.LogError("RemoteModelAsset == null");
                
                modelAsset = Object.Instantiate(modelAsset);
                return modelAsset;
            }
        }
        
        public static string ApplicationUrl
        {
            get
            {
                if (!string.IsNullOrEmpty(_remoteAppUrl))
                    return _remoteAppUrl;
                
                var asset = ModelAsset;
                if (asset == null) return EmptyPath;
                
                var url = asset.data.gameDataUrl;
#if UNITY_WEBGL
                if(!Application.isEditor)
                    url = Application.absoluteURL;
                url = url.Replace("index.html",string.Empty);
#endif
                url = url.TrimEndPath();
                _remoteAppUrl = url;
                return _remoteAppUrl;
            }
        }
        
        #endregion
        
        #region inspector 
        
        [InlineProperty]
        [HideLabel]
        [OnValueChanged(nameof(RefreshPreview))]
        [OnInspectorInit(nameof(RefreshPreview))]
        public RemoteGameModel data = new();

        [NonSerialized]
        [OnInspectorGUI]
        [ReadOnly]
        private string _demoUrl = string.Empty;
        
        #endregion

        public void ParseData(string jsonData)
        {
            var modelData = JsonConvert.DeserializeObject<RemoteGameModel>(jsonData);
            if (modelData == null) return;
            
            ModelAsset.data = modelData;
            Reset();
        }

        public static string GetAddressableRemoteUrl(RemoteGameModel model)
        {
            if (model == null) return EmptyPath;
            if (!string.IsNullOrEmpty(_remoteStreamingAssetDataUrl)) 
                return _remoteStreamingAssetDataUrl;

            if (model.overrideAddressableUrl)
                return model.addressableUrl;
            
            var url = ApplicationUrl;

            if (!model.useRelatedUrl)
            {
                if (model.addBuildTarget)
                {
#if UNITY_EDITOR
                    url = $"{url}/{EditorUserBuildSettings.activeBuildTarget.ToString()}";
#else
                url = $"{url}/{Application.platform.ToString()}";
#endif
                }

                url = url.TrimEndPath();
                
                if(model.addGameVersion)
                    url = $"{url}/{Application.version}";
            }
                
            if(!string.IsNullOrEmpty(model.addressablePath))
                url = $"{url}/{model.addressablePath}";

            return url;
        }
    }
}