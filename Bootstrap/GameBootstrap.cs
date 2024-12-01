namespace Game.Runtime.Services.Bootstrap
{
    using System;
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using GameSettings;
    using Runtime.Bootstrap;
    using Tools;
    using UniCore.Runtime.ProfilerTools;
    using UniGame.AddressableTools.Runtime;
    using UniGame.Context.Runtime.DataSources;
    using UniGame.Core.Runtime;
    using UniModules.UniCore.Runtime.DataFlow;
    using UniModules.UniGame.Context.Runtime.Context;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using UnityEngine.SceneManagement;
    using Object = UnityEngine.Object;

    public static class GameBootstrap
    {
        private static Dictionary<string, Func<IContext, UniTask<bool>>> _bootStages
            = new()
            {
                { nameof(SettingAddressableParametersAsync), SettingAddressableParametersAsync },
                { nameof(InitializeAddressableAsync), InitializeAddressableAsync },
                { nameof(CheckAssetBundleCache), CheckAssetBundleCache },
                { nameof(InitializeAsync), InitializeAsync },
                { nameof(LoadBootSceneAsync), LoadBootSceneAsync },
                { nameof(InitializeServicesAsync), InitializeServicesAsync },
            };

        private static LifeTimeDefinition _lifeTime;
        private static EntityContext _context;
        private static GameBootSettings _settings;

        public static ILifeTime LifeTime => _lifeTime;
        public static IContext Context => _context;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void InitializeGame()
        {
            InitializeInnerAsync().Forget();
        }

        public static void Restart()
        {
            InitializeInnerAsync().Forget();
        }


        public static void Dispose()
        {
            _lifeTime?.Terminate();
        }

        private static async UniTask<bool> CheckAssetBundleCache(IContext lifeTime)
        {
            var version = PlayerPrefs.GetString(nameof(Application.version), string.Empty);
            if(string.Equals(version, Application.version)) return true;
            
            PlayerPrefs.SetString(nameof(Application.version), Application.version);

#if !UNITY_WEBGL
            Caching.ClearCache();
#endif
            
            return true;
        }

        private static async UniTask<bool> SettingAddressableParametersAsync(IContext lifeTime)
        {
            RemoteModelAsset.Reset();
            
            var fileName = "AddressableSettings.json";
            var loadData = await StreamingAssetsLoader.LoadDataFromWeb(fileName);

            if (loadData.success)
                RemoteModelAsset.ModelAsset.ParseData(loadData.data);
            
            return true;
        }

        private static async UniTask<bool> InitializeAddressableAsync(IContext lifeTime)
        {
            await Addressables.InitializeAsync();
            GameLog.Log("Addressable initialized");
            return true;
        }
        
        private static async UniTask<bool> LoadBootSceneAsync(IContext lifeTime)
        {
            var bootSceneEnabled = _settings.bootSceneEnabled;
#if UNITY_EDITOR
            bootSceneEnabled = false;
#endif
            if (!bootSceneEnabled) return true;
            
            _settings.bootScene
                .LoadSceneTaskAsync(lifeTime.LifeTime, LoadSceneMode.Single,true)
                .Forget();
            
            return true;
        }


        private static async UniTask<bool> InitializeAsync(IContext context)
        {
            var settingsAssetResult = await nameof(GameBootSettings)
                    .LoadAssetTaskAsync<GameBootSettings>(context.LifeTime)
                    .SuppressCancellationThrow();

            if (settingsAssetResult.IsCanceled ||
                settingsAssetResult.Result == null)
                return false;

            var result = settingsAssetResult.Result;
            _settings = Object.Instantiate(result);

            return true;
        }
        
        private static async UniTask<bool> InitializeServicesAsync(IContext context)
        {
            var lifeTime = context.LifeTime;
            var source = await _settings.sources
                .LoadAssetInstanceTaskAsync<AsyncDataSources>(lifeTime, true);
            
            source.AddTo(lifeTime);
            
            await source.RegisterAsync(_context);

            return true;
        }

        private static async UniTask InitializeInnerAsync()
        {
            _lifeTime?.Release();
            _lifeTime = new LifeTimeDefinition();
            _context = new EntityContext().AddTo(_lifeTime);
            
            GameContext.Context = _context;
            
            foreach (var stage in _bootStages)
            {
                var stageName = stage.Key;
                var stageFunc = stage.Value;

                GameLog.Log($"Game Boot STAGE Execute: {stageName}");

                var stageResult = await stageFunc.Invoke(_context);
                if (!stageResult)
                {
                    Debug.LogError($"Game Boot STAGE ERROR: {stageName}");
                    return;
                }

                GameLog.Log($"Game Boot STAGE Complete: {stageName}");
            }
        }
    }
}