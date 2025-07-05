namespace Game.Runtime.Services.Bootstrap
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using GameSettings;
    using Runtime.Bootstrap;
    using Tools;
    using UniCore.Runtime.ProfilerTools;
    using UniGame.AddressableTools.Runtime;
    using UniGame.Context.Runtime;
    using UniGame.Core.Runtime;
    using UniGame.Runtime.DataFlow;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using Object = UnityEngine.Object;

    public static class GameBootstrap
    {
        private static Dictionary<string, Func<IContext, UniTask<bool>>> _bootStages = new()
            {
                { nameof(SettingAddressableParametersAsync), SettingAddressableParametersAsync },
                { nameof(InitializeAddressableAsync), InitializeAddressableAsync },
                { nameof(CheckAssetBundleCache), CheckAssetBundleCache },
                { nameof(InitializeAsync), InitializeAsync },
                { nameof(ExecuteBootStepsAsync), ExecuteBootStepsAsync },
                { nameof(InitializeServicesAsync), InitializeServicesAsync },
            };

        private static LifeTime _lifeTime;
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
            _lifeTime?.Release();
        }
        
        private static async UniTask InitializeInnerAsync()
        {
            _lifeTime?.Release();
            _lifeTime = new ();
            _context = new EntityContext();
            _context.AddTo(_lifeTime);
            
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

        private static async UniTask<bool> CheckAssetBundleCache(IContext lifeTime)
        {
            var version = PlayerPrefs.GetString(nameof(Application.version), string.Empty);
            if (string.Equals(version, Application.version)) return true;

            PlayerPrefs.SetString(nameof(Application.version), Application.version);

#if !UNITY_WEBGL
            Caching.ClearCache();
#endif

            return true;
        }

        private static async UniTask<bool> SettingAddressableParametersAsync(IContext lifeTime)
        {
            RemoteModelAsset.Reset();

            var fileName = RemoteModelAsset.RemoteSettingsName;
            var loadData = await StreamingAssetsUtils.LoadDataFromWeb(fileName);

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
        
        private static async UniTask<bool> InitializeAsync(IContext context)
        {
            var settingsResource = await nameof(GameBootSettings)
                .LoadAssetTaskAsync<GameBootSettings>(context.LifeTime);
            
            if (settingsResource == null)
            {
                settingsResource = Resources.Load<GameBootSettings>(string.Empty);
            }

            if (settingsResource == null)
                return false;

            var result = settingsResource;
            _settings = Object.Instantiate(result);

            return true;
        }

        private static async UniTask<bool> ExecuteBootStepsAsync(IContext context)
        {
            foreach (var bootCommand in _settings.bootCommands)
            {
                var result = await bootCommand.ExecuteAsync(context);
                if (result.success) continue;

                Debug.LogError($"GameBootstrap: BootCommand Error {bootCommand.GetType().Name} Error: {result.error}");
                if (result.canContinue) continue;

                return false;
            }

            return true;
        }

        private static async UniTask<bool> InitializeServicesAsync(IContext context)
        {
            var sources = _settings.dataSources
                .Select(x => x.RegisterAsync(context))
                .Concat(Enumerable.Repeat(_settings.defaultSource.RegisterAsync(context), 1));
            
            await UniTask.WhenAll(sources);

            return true;
        }
    }
}