namespace Game.Code.Services.GameSettingsService
{
    using System;
    using Sirenix.OdinInspector;
    using UniModules.UniGame.Core.Runtime.Rx;

    [Serializable]
    public class GameSettingsModel
    {
        [DrawWithUnity]
        public ReactiveValue<bool> profiler = new();
        
        [DrawWithUnity]
        public  ReactiveValue<bool>  console = new();
        
        [DrawWithUnity]
        public  ReactiveValue<int>  frameRate = new(40);
        
        [DrawWithUnity]
        public  ReactiveValue<float>  screenScale = new();
        
        [DrawWithUnity]
        public  ReactiveValue<float>  dpiScale = new();
    }
}