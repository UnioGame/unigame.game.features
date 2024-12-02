namespace Game.Runtime.Services.WebService
{
    using System.IO;
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using global::Modules.WebServer;
    using global::Modules.WebTexture;
    using Newtonsoft.Json;
    using Sirenix.OdinInspector;
    using Tools;
    using UniCore.Runtime.ProfilerTools;
    using UniGame.AddressableTools.Runtime;
    using UniGame.Context.Runtime.Extension;
    using UniGame.Core.Runtime;
    using UniGame.GameFlow.Runtime.Services;
    using UniGame.MetaBackend.Shared;
    using UniModules.UniGame.Core.Runtime.DataFlow.Extensions;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using Object = UnityEngine.Object;

    /// <summary>
    /// Настройки для общения с сервером
    /// </summary>
    [CreateAssetMenu(menuName = "Game/Services/Web/WebService Source", fileName = "WebService Source")]
    public class WebServiceSource : DataSourceAsset<IWebService>
    {
        [InlineEditor]
        public WebSettingsAsset settingsAsset;

        protected override async UniTask<IWebService> CreateInternalAsync(IContext context)
        {
            var settingsData = Instantiate(settingsAsset);
            var configuration = settingsData.configuration;
            var receiver = await SpawnAsset<WebMessageReceiver>(configuration.messageReceiver);
            var settings = await LoadWebSettingsAsync(configuration);
            
            context.Publish(configuration);
            
            var metaService = await context.ReceiveFirstAsync<IBackendMetaService>();
            var webMetaProvider = await context.ReceiveFirstAsync<IWebMetaProvider>();
            var textureProvider = await context.ReceiveFirstAsync<IWebTextureProvider>();
            
            var service = new WebService(metaService,
                webMetaProvider,
                textureProvider,
                settings, configuration.offline, receiver);
            
            receiver.InitializeAsync(settings.timeout).Forget();
            return service;
        }

        private async UniTask<WebSettings> LoadWebSettingsAsync(WebServiceConfiguration configuration)
        {
#if UNITY_EDITOR
            var loadFromStreaming = configuration.useStreamingAssetsUnderEditor && configuration.useStreamingAssets;
#else
            var loadFromStreaming = configuration.useStreamingAssets;
#endif
            
            var result = await LoadWebSettingsFromConfigsAsync(configuration);

            if (loadFromStreaming)
            {
                result = await LoadWebSettingsFromStreamingAsync(configuration,result);
            }

            return result;
        }
        
        private async UniTask<WebSettings> LoadWebSettingsFromConfigsAsync(WebServiceConfiguration configuration)
        {
            var id = configuration.selectedSettings;
            var selectedSettings = configuration
                .settings
                .FirstOrDefault(x => x.settingsId == id);

            var settings = await selectedSettings
                .reference
                .LoadAssetInstanceTaskAsync(LifeTime, true);

            return settings;
        }

        private async UniTask<WebSettings> LoadWebSettingsFromStreamingAsync(WebServiceConfiguration configuration,WebSettings settings)
        {
            return UpdateSettings(settings, await LoadSettings(configuration));
        }

        private WebSettings UpdateSettings(WebSettings settings, WebSettingsPublicData data)
        {
            settings.apiURL = data.apiURL;
            settings.timeout = data.timeoutInSeconds;
            return settings;
        }

        private async UniTask<WebSettingsPublicData> LoadSettings(WebServiceConfiguration configuration)
        {
            var loadFromWeb = false;
#if UNITY_WEBGL
            loadFromWeb = true;  
#endif
            var result = loadFromWeb 
                ? await StreamingAssetsLoader.LoadDataFromWeb(configuration.webServiceSettingsPath)
                : await StreamingAssetsLoader.LoadDataFromFile(configuration.webServiceSettingsPath);
            
            var data = JsonConvert.DeserializeObject<WebSettingsPublicData>(result.data);
            return data ?? new WebSettingsPublicData();
        }


        private async UniTask<T> SpawnAsset<T>(AssetReferenceT<GameObject> reference) where T : Object
        {
            var asset = await reference.LoadAssetTaskAsync(LifeTime);
            var instance = Instantiate(asset);

            if (instance == null) return default;

            instance.SetActive(true);
            DontDestroyOnLoad(instance);

            return instance.GetComponent<T>();
        }
    }
}