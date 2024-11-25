namespace Game.Code.Services.GameSettingsService
{
    using System;
    using Services.GameSettings.Data;
    using UniGame.UniNodes.GameFlow.Runtime;
    using UniRx;
    using UnityEngine;

    [Serializable]
    public class GameSettingsService : GameService, IGameSettingsService
    {
        private GameSettings _settings;
        private GameSettingsModel _model;
        
        private ScreenResolution _defaultResolution;
        private ScreenResolution _activeResolution;
        private Resolution _defaultScreen;
        private int _defaultFrameRate;

        public GameSettingsService(GameSettings defaultData)
        {
            _settings = defaultData;
            _model = new GameSettingsModel();
            
            var frameRate = Application.isEditor 
                ? _settings.editorTargetFrameRate 
                : _settings.targetFrameRate;
            
            _model.frameRate.Value = frameRate;
            _model.screenScale.Value = _settings.screenScale;
            _model.profiler.Value = _settings.profiler;
            _model.console.Value = _settings.console;
            _model.dpiScale.Value = _settings.dpiScale;
            
            _defaultFrameRate = Application.targetFrameRate;
            _defaultScreen = Screen.currentResolution;
            _defaultResolution = GetScreenResolution(ref _defaultScreen);
            _activeResolution = _defaultResolution;
            
            SetFrameRate(frameRate);
            SetDpiScale(_model.dpiScale.Value);
            
            _model.frameRate
                .Subscribe(SetFrameRate)
                .AddTo(LifeTime);

            if (defaultData.enableScreenScale)
            {
                SetScreenScale(_model.screenScale.Value);
                _model.screenScale
                    .Subscribe(SetScreenScale)
                    .AddTo(LifeTime);
            }
            
            _model.dpiScale
                .Subscribe(SetDpiScale)
                .AddTo(LifeTime);
        }
        
        public ScreenResolution DefaultResolution => _defaultResolution;
        public ScreenResolution ActiveResolution => _activeResolution;

        public IReactiveProperty<bool> Profiler => _model.profiler;
        public IReactiveProperty<bool> Console => _model.console;
        public IReactiveProperty<int> FrameRate => _model.frameRate;
        public IReactiveProperty<float> ScreenScale => _model.screenScale;
        public IReactiveProperty<float> DpiScale => _model.dpiScale;
        
        public static void SetFrameRate(int frameRate)
        {
            Application.targetFrameRate = frameRate;
        }
        
        public static void SetDpiScale(float scale)
        {
            var targetScale = Mathf.Clamp(scale, 0.1f, 1f);
            QualitySettings.resolutionScalingFixedDPIFactor = targetScale;
        }

        public void SetScreenScale(float targetScale)
        {
            var minSize = _settings.minScreenSize;
            var minSide = Mathf.Min(_defaultResolution.width, _defaultResolution.height);
            var isLessThanMin = minSize > 0 && minSize < minSide * targetScale;
            if (isLessThanMin)
                targetScale = minSize / (float)minSide;
            
            var scale = Mathf.Clamp(targetScale, 0.1f, 1f);
                
            var newWidth = _defaultResolution.width * scale;
            var newHeight =  _defaultResolution.height * scale;
            
            _activeResolution.width = (int)newWidth;
            _activeResolution.height = (int)newHeight;
            
            SetScreenResolution(ref _activeResolution);
        }

        public void Reset()
        {
            Application.targetFrameRate = _defaultFrameRate;
            ResetScreenResolution();
            SetDpiScale(1f);
        }
        
        public void ResetScreenResolution()
        {
            SetScreenResolution(ref _defaultResolution,true);
        }
        
        
        public void SetScreenResolution(int width, int height)
        {
            var resolution = Screen.currentResolution;
            var refreshRate = resolution.refreshRateRatio;
            
            Screen.SetResolution(width, height, Screen.fullScreenMode,refreshRate);

            _activeResolution = GetScreenResolution(ref resolution);
        }
        
        public void SetScreenResolution(ref ScreenResolution screenResolution,bool setRefreshRate = false)
        {
            var resolution = Screen.currentResolution;
            var refreshRate = setRefreshRate ? screenResolution.refreshRate : resolution.refreshRateRatio;
            
            Screen.SetResolution(screenResolution.width, screenResolution.height,
                Screen.fullScreenMode,refreshRate);

            _activeResolution = GetScreenResolution(ref resolution);
        }
        
        public static ScreenResolution GetScreenResolution(ref Resolution source)
        {
            var target = new ScreenResolution
            {
                dpi = Screen.dpi,
                refreshRate = source.refreshRateRatio,
                width = source.width,
                height = source.height,
                aspect = source.width/ (float)source.height
            };

            return target;
        }
    }
}