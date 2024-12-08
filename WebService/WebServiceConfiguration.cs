namespace Game.Runtime.Services.WebService
{
    using System;
    using System.Collections.Generic;
    using Sirenix.OdinInspector;
    using UnityEngine;
    using UnityEngine.AddressableAssets;

    [Serializable]
    public class WebServiceConfiguration
    {
        public bool useStreamingAssetsUnderEditor = false;
        public bool useStreamingAssets = true;

        [ShowIf(nameof(useStreamingAssets))]
        public string webServiceSettingsPath = "web_service_settings.json";

        public bool offline;
        
        [ValueDropdown(nameof(GetSettings))]
        public string selectedSettings;
        
        public AssetReferenceT<GameObject> messageReceiver;
        
        public List<WebSettingReference> settings = new();
        
        public IEnumerable<ValueDropdownItem<string>> GetSettings()
        {
            foreach (var setting in settings)
            {
                var item = new ValueDropdownItem<string>()
                {
                    Text = setting.settingsId,
                    Value = setting.settingsId,
                };

                yield return item;
            }
        }
    }
}