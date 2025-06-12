namespace Game.Runtime.Services.RemoteSettingsService.Editor
{
    using System;
    using System.IO;
    using GameSettings;
    using Newtonsoft.Json;
    using Sirenix.OdinInspector;
    using UniGame.UniBuild.Editor.ClientBuild.Interfaces;
    using UniGame.UniBuild.Editor.Commands.PreBuildCommands;
    using UniModules.Editor;
    using UnityEngine;

    [Serializable]
    public class UpdateRemoteSettingsCommand : SerializableBuildCommand
    {
        [InlineProperty]
        [HideLabel]
        public RemoteGameModel remoteData = new();

        [NonSerialized]
        [OnInspectorGUI]
        [ReadOnly]
        [OnInspectorInit(nameof(RefreshPreview))]
        private string _demoUrl = string.Empty;
        
        [Button]
        public void RefreshPreview()
        {
            _demoUrl = RemoteModelAsset.GetAddressableRemoteUrl(this.remoteData);
        }
        
        public override void Execute(IUniBuilderConfiguration buildParameters)
        {
            Execute();
        }

        [Button]
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