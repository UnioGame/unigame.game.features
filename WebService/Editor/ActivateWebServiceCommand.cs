namespace Game.Runtime.Services.WebService.Editor
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Sirenix.OdinInspector;
    using UniGame.UniBuild.Editor.ClientBuild.Interfaces;
    using UniModules;
    using UniModules.Editor;
    using UniModules.UniGame.UniBuild.Editor.ClientBuild.Commands.PreBuildCommands;
    using UnityEditor;
    using UnityEngine;

    [Serializable]
    public class ActivateWebServiceCommand : SerializableBuildCommand
    {
        [ValueDropdown(nameof(GetSettings))]
        public string settingId = string.Empty;
        public bool isOffline = false;
        public bool useStreamingAssets = true;
        
        public override void Execute(IUniBuilderConfiguration buildParameters)
        {
            Execute();
        }

        [Button]
        public void Execute()
        {
            var serviceAsset = AssetEditorTools.GetAsset<WebServiceSource>();
            if(serviceAsset == null)
                return;

            var configurationAsset = serviceAsset.settingsAsset;
            var configuration = configurationAsset.configuration;
            configuration.offline = isOffline;
            
            var settings = configuration;
            var settingValue = settings.settings.FirstOrDefault(x => x.settingsId == settingId);
            
            if (settingValue == null)
            {
                Debug.LogError($"{nameof(ActivateWebServiceCommand)} setting not found {settingId}");
                return;
            }
            
            var settingsAsset = settingValue.EditorValue;
            configuration.useStreamingAssets = useStreamingAssets;
            
            if (useStreamingAssets)
            {
                var streamingPath = Application.streamingAssetsPath;
                var settingsPath = streamingPath.CombinePath(configuration.webServiceSettingsPath);
                var json = settingsAsset.SerializeToJson();
                File.WriteAllText(settingsPath,json);
                
                AssetDatabase.ImportAsset(settingsPath);
            }
            
            configuration.selectedSettings = settingId;
            configurationAsset.SaveAsset();
            serviceAsset.SaveAsset();
        }
        
        public IEnumerable<ValueDropdownItem<string>> GetSettings()
        {
            var serviceAsset = AssetEditorTools.GetAsset<WebServiceSource>();
            if(serviceAsset == null)
                yield break;

            var settings = serviceAsset.settingsAsset.configuration;
            foreach (var setting in settings.GetSettings())
                yield return setting;
        }
    }
}