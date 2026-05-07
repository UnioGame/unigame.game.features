namespace Game.Runtime.Services.RemoteSettingsService.Editor
{
    using System;
    using System.IO;
    using GameSettings;
    using Newtonsoft.Json;
    
    using UniGame.UniBuild.Editor;
    using UniGame.UniBuild.Editor.Commands;
    using UniModules.Editor;
    using UnityEngine;
    
#if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
#endif

    [Serializable]
    public class UpdateRemoteSettingsCommand : SerializableBuildCommand
    {
#if ODIN_INSPECTOR
        [InlineProperty]
        [HideLabel]
#endif

        public RemoteGameModel remoteData = new();

        [NonSerialized]
#if ODIN_INSPECTOR
        [OnInspectorGUI]
        [ReadOnly]
        [OnInspectorInit(nameof(RefreshPreview))]
#endif

        private string _demoUrl = string.Empty;
        
#if ODIN_INSPECTOR
        [Button]
#endif
        public void RefreshPreview()
        {
            _demoUrl = RemoteModelAsset.GetAddressableRemoteUrl(this.remoteData);
        }
        
        public override void Execute(IUniBuilderConfiguration buildParameters)
        {
            Execute();
        }

#if ODIN_INSPECTOR
        [Button]
#endif
        public void Execute()
        {
            var settings = AssetEditorTools.GetAsset<RemoteModelAsset>();

            var remoteValue = JsonConvert.SerializeObject(remoteData);
            var message = $"BUILD: Update Remote Settings = {settings.name} with new data \n\t {remoteValue}";
            Debug.Log(message);
            
            settings.data = remoteData;
            settings.SaveAsset();
            
            Execute(settings);
            
            settings.SaveAsset();
        }

        public void Execute(RemoteModelAsset remoteModelAsset)
        {
            remoteModelAsset.ResetRemote();
            
            var path = Path.Combine(Application.streamingAssetsPath,RemoteModelAsset.RemoteSettingsName);
            var jsonData = JsonConvert.SerializeObject(remoteData, Formatting.Indented);
            
            File.WriteAllText(path, jsonData);
        }
    }
}