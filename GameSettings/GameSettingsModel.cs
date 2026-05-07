namespace Game.Code.Services.GameSettingsService
{
    using System;
    using UniGame.Runtime.Rx;

#if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
#endif
    
    [Serializable]
    public class GameSettingsModel
    {
#if ODIN_INSPECTOR
        [DrawWithUnity]
#endif
        public ReactiveValue<bool> profiler = new();
        
#if ODIN_INSPECTOR
        [DrawWithUnity]
#endif
        public  ReactiveValue<bool>  console = new();
        
#if ODIN_INSPECTOR
        [DrawWithUnity]
#endif
        public  ReactiveValue<int>  frameRate = new(40);
        
#if ODIN_INSPECTOR
        [DrawWithUnity]
#endif
        public  ReactiveValue<float>  screenScale = new();
        
#if ODIN_INSPECTOR
        [DrawWithUnity]
#endif
        public  ReactiveValue<float>  dpiScale = new();
    }
}