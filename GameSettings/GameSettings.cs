namespace Game.Code.Services.GameSettingsService
{
    using System;
    using UnityEngine;

#if ODIN_INSPECTOR
        using Sirenix.OdinInspector;
#endif
    
    [Serializable]
    public class GameSettings
    {
        public bool profiler = false;
        
        [Tooltip("target frame rate for game")]
        public bool console = false;
        
        [Tooltip("target frame rate for game")]
        public int targetFrameRate = 60;
        
        [Tooltip("target frame under editor for game")]
        public int editorTargetFrameRate = 120;
        
        [Tooltip("enable screen scale for perfomance optimization")]
        public bool enableScreenScale = false;
        
#if ODIN_INSPECTOR
        [ShowIf(nameof(enableScreenScale))]
#endif
        [Tooltip("screen scale for perfomance optimization")]
        [Range(0.2f, 1.0f)]
        public float screenScale = 1.0f;
        
        [Tooltip("minimum screen size")]
        public int minScreenSize = 720;
        
        [Tooltip("this factor is used to multiply with the target Fixed DPI")]
        [Range(0.2f, 1.0f)]
        public float dpiScale = 1.0f;
    }
}