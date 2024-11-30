namespace Game.Runtime.Services.DebugService
{
    using System;
    using Code.Services.GameSettingsService;
    using Cysharp.Threading.Tasks;
    using Sirenix.OdinInspector;
    using UniGame.Context.Runtime.Extension;
    using UniGame.Core.Runtime;
    using UniGame.GameFlow.Runtime.Services;
    using UniRx;
    using UnityEngine;

    /// <summary>
    /// game debug
    /// </summary>
    [CreateAssetMenu(menuName = "Game/Services/DebugService/Debug Source", fileName = "DebugService Source")]
    public class DebugServiceSource : DataSourceAsset<IDebugService>
    {
        [InlineProperty]
        [HideLabel]
        public DebugConfiguration debugConfiguration = new();
        
        protected override async UniTask<IDebugService> CreateInternalAsync(IContext context)
        {
            var service = new DebugService(debugConfiguration);
            
            InitializeDebugAsync(context,service).Forget();
            
            return service;
        }

        private async UniTask InitializeDebugAsync(IContext context,IDebugService service)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(0));
            
            var settingsModel = await context.ReceiveFirstAsync<IGameSettingsService>();

            service.SetVersionState(true);
            
            settingsModel.Profiler
                .Do(x => Debug.Log($"Debug Service Activate Profiler: {x}"))
                .Subscribe(service.SetProfilerState)
                .AddTo(context.LifeTime);
            
            settingsModel.Console
                .Do(x => Debug.Log($"Debug Service Activate Console: {x}"))
                .Subscribe(service.SetConsoleState)
                .AddTo(context.LifeTime);
        }
    }

}