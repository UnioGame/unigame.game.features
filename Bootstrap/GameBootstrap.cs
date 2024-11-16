namespace Game.Runtime.Services.Bootstrap
{
    using System;
    using System.Collections.Generic;
    using AddressablesSource;
    using Cysharp.Threading.Tasks;
    using Game.GameSettings;
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

    public static class GameBootstrap
    {
        private static Dictionary<string, Func<ILifeTime, UniTask<bool>>> _bootStages
            = new()
            {
                { nameof(OnSetupAsync), OnSetupAsync },
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

        private static async UniTask<bool> CheckAssetBundleCache(ILifeTime lifeTime)
        {
            var version = PlayerPrefs.GetString(nameof(Application.version), string.Empty);
            if(string.Equals(version, Application.version)) return true;
            
            PlayerPrefs.SetString(nameof(Application.version), Application.version);

#if !UNITY_WEBGL
            Caching.ClearCache();
#endif
            
            return true;
        }

        private static async UniTask<bool> SettingAddressableParametersAsync(ILifeTime lifeTime)
        {
            RemoteModelAsset.Reset();
            
            var fileName = "AddressableSettings.json";
            var loadData = await StreamingAssetsLoader.LoadDataLikeWeb(fileName);

            if (loadData.success)
                RemoteModelAsset.ModelAsset.ParseData(loadData.data);
            
            return true;
        }

        private static async UniTask<bool> InitializeAddressableAsync(ILifeTime lifeTime)
        {
            await Addressables.InitializeAsync();
            GameLog.Log("Addressable initialized");
            return true;
        }
        
        private static async UniTask<bool> LoadBootSceneAsync(ILifeTime lifeTime)
        {
            var bootSceneEnabled = _settings.bootSceneEnabled;
#if UNITY_EDITOR
            bootSceneEnabled = false;
#endif
            if (!bootSceneEnabled) return true;
            
            _settings.bootScene
                .LoadSceneTaskAsync(lifeTime, LoadSceneMode.Single,true)
                .Forget();
            
            return true;
        }

        private static UniTask<bool> OnSetupAsync(ILifeTime lifeTime)
        {
            _lifeTime ??= new LifeTimeDefinition();
            _lifeTime.Release();
            _context = new EntityContext().AddTo(_lifeTime);
            
            GameContext.Context = _context;
            
            return UniTask.FromResult(true);
        }

        private static async UniTask<bool> InitializeAsync(ILifeTime lifeTime)
        {
            var settingsAssetResult = await
                nameof(GameBootSettings)
                    .LoadAssetTaskAsync<GameBootSettings>(lifeTime)
                    .SuppressCancellationThrow();

            if (settingsAssetResult.IsCanceled)
                return false;

            var settingsAsset = settingsAssetResult.Result;
            var result = settingsAsset.Result;
            _settings = result;

            return true;
        }
        
        private static async UniTask<bool> InitializeServicesAsync(ILifeTime lifeTime)
        {
            var source = await _settings.sources.LoadAssetInstanceTaskAsync<AsyncDataSources>(lifeTime, true);
            source.AddTo(lifeTime);
            await source.RegisterAsync(_context);

            return true;
        }

        private static async UniTask InitializeInnerAsync()
        {
            foreach (var stage in _bootStages)
            {
                var stageName = stage.Key;
                var stageFunc = stage.Value;

                GameLog.Log($"Game Boot STAGE Execute: {stageName}");

                var stageResult = await stageFunc.Invoke(_lifeTime);
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