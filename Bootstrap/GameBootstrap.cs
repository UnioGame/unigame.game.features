namespace Game.Runtime.Services.Bootstrap
{
    using System;
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using Runtime.Bootstrap;
    using UniCore.Runtime.ProfilerTools;
    using UniGame.AddressableTools.Runtime;
    using UniGame.Context.Runtime;
    using UniGame.Core.Runtime;
    using UniGame.Runtime.DataFlow;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using UniGame.Features.Bootstrap;
    using Object = UnityEngine.Object;

    public static class GameBootstrap
    {
        private static Dictionary<string, Func<IContext, UniTask<bool>>> _bootStages = new()
        {
            { nameof(InitializeAddressableAsync), InitializeAddressableAsync },
            { nameof(InitializeAsync), InitializeAsync },
            { nameof(ExecuteBootInitStepsAsync), ExecuteBootInitStepsAsync },
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
            _lifeTime?.Terminate();
        }

        private static async UniTask InitializeInnerAsync()
        {
            _lifeTime?.Terminate();
            _lifeTime = new();
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

        private static async UniTask<bool> ExecuteBootInitStepsAsync(IContext context)
        {
            return await ExecuteBootStepsAsync(_settings.gameInitCommands, context);
        }

        private static async UniTask<bool> ExecuteBootStepsAsync(List<IGameBootCommand> bootCommands, IContext context)
        {
            foreach (var bootCommand in bootCommands)
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
            if (_settings.source.enabled == false)
            {
                GameLog.Log("GameBootstrap: Game Sources disabled");
                return true;
            }

            try
            {
                var sources = await _settings
                    .source
                    .RegisterAsync(context);
            }
            catch (Exception e)
            {
                GameLog.LogException(e);
                return false;
            }

            return true;
        }
    }
}