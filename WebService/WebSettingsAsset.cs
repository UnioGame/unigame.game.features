namespace Game.Runtime.Services.WebService
{
    using System.IO;
    using System.Linq;
    using Sirenix.OdinInspector;
    using UniCore.Runtime.ProfilerTools;
    using UnityEngine;
    using UnityEngine.AddressableAssets;

    [CreateAssetMenu(menuName = "Game/Services/Web/WebSettings Asset", fileName = "WebSettings Asset")]
    public class WebSettingsAsset : ScriptableObject
    {

        [InlineProperty]
        [TitleGroup("Settings")]
        [HideLabel]
        public WebServiceConfiguration configuration = new();
        
#if UNITY_EDITOR
        [TitleGroup("Export")]
        [PropertyOrder(-1)]
        [PropertySpace(SpaceBefore = 5, SpaceAfter = 10)]
        [Button(ButtonSizes.Large, Icon = SdfIconType.Save2, Name = "Export Selected Settings to JSON")]
        public void ExportSettings()
        {
            var id = configuration.selectedSettings;
            var selectedSettings = configuration
                .settings
                .FirstOrDefault(x => x.settingsId == id);
            var settingsAsset = selectedSettings.reference.editorAsset;
            var json = settingsAsset.SerializeToJson();
            var path = Path.Combine(Application.streamingAssetsPath, configuration.webServiceSettingsPath);
            File.WriteAllText(path, json);
            GameLog.Log("Settings exported to: " + path, Color.green);
#endif
            
        }
        

    }
}