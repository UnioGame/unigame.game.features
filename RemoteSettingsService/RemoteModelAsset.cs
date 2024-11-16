namespace Game.GameSettings
{
    using System;
    using Newtonsoft.Json;
    using Runtime;
    using Sirenix.OdinInspector;
    using UniModules;
    using UnityEngine;
    
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
        private static string _remoteDefaultDataUrl = string.Empty;
        private static string _remoteStreamingAssetDataUrl = string.Empty;
        private static string _buildTargetUrl = string.Empty;
        private static string _remoteVersionUrl = string.Empty;

        public const string EmptyPath = "[EMPTY_REMOTE_ASSET]";
        
        [InlineProperty]
        [HideLabel]
        [OnValueChanged(nameof(RefreshPreview))]
        [OnInspectorInit(nameof(RefreshPreview))]
        public RemoteGameModel data = new();

        [NonSerialized]
        [OnInspectorGUI]
        [ReadOnly]
        private string _demoUrl = string.Empty;
        
        [Button]
        public void ResetRemote()
        {
            Reset();
        }

        [Button]
        public void RefreshPreview()
        {
            _demoUrl = GetRemoteUrl(this.data);
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
        }

        public static string BundleUrl
        {
            get
            {
                if (!string.IsNullOrEmpty(_remoteDefaultDataUrl))
                    return _remoteDefaultDataUrl;
                
                var asset = ModelAsset;
                if (asset == null) return EmptyPath;
                
                var url = GetRemoteUrl(asset.data);
                
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
                return modelAsset;
            }
        }
        
        public void ParseData(string data)
        {
            var json = JsonConvert.DeserializeObject<RemoteGameModel>(data);
            
            if (json == null) return;
            
            _remoteStreamingAssetDataUrl = json.gameDataUrl;
        }

        public static string GetRemoteUrl(RemoteGameModel model)
        {
            if (model == null) return EmptyPath;
            if (!string.IsNullOrEmpty(_remoteStreamingAssetDataUrl)) return _remoteStreamingAssetDataUrl;
            var url = model.gameDataUrl;
            url = url.TrimEndPath();

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
                
            if(!string.IsNullOrEmpty(model.remoteFolder))
                url = $"{url}/{model.remoteFolder}";

            return url;
        }
    }
}