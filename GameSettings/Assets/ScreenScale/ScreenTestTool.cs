using UnityEngine;

namespace GameSettings.Test
{
    using Cysharp.Threading.Tasks;
    using Game.Code.Services.GameSettingsService;
    using Sirenix.OdinInspector;
    using TMPro;
    using UniGame.Context.Runtime.Extension;
    using UniModules.UniGame.Context.Runtime.Context;
    using UnityEngine.UI;

    public class ScreenTestTool : MonoBehaviour
    {
        public TextMeshProUGUI info;
        public TMP_InputField x;
        public TMP_InputField y;
        public TMP_InputField scale;
        public TMP_InputField dpi;
        public TMP_InputField fps;

        public Button apply;

        public Resolution resolution;
        
        private IGameSettingsService _gameSettingsService;

        private void Awake()
        {
            apply.onClick.AddListener(Apply);
            InitializeAsync().Forget();
        }

        private async UniTask InitializeAsync()
        {
            var context =await GameContext.GetContextAsync(destroyCancellationToken);
            _gameSettingsService = await context
                .ReceiveFirstAsync<IGameSettingsService>();
            
            UpdateInfo();
        }

        public void UpdateInfo()
        {
            if(_gameSettingsService == null)
                return;
            
            x.text = resolution.width.ToString();
            y.text = resolution.height.ToString();
            
            fps.text = _gameSettingsService.FrameRate.Value.ToString();
            dpi.text = _gameSettingsService.DpiScale.Value.ToString();
            scale.text = _gameSettingsService.ScreenScale.Value.ToString();
        }

        private void Update()
        {
            resolution = Screen.currentResolution;
            var resolutionNow = GameSettingsService.GetScreenResolution(ref resolution);
            
            info.text = $"Default: {_gameSettingsService.DefaultResolution}\n" +
                        $"Resolution: {_gameSettingsService.ActiveResolution}\n" +
                        $"Now: {resolutionNow}\n" +
                        $"Screen: {_gameSettingsService.ActiveResolution}\n" +
                        $"DPI: {_gameSettingsService.DpiScale.Value}\n" +
                        $"FPS: {_gameSettingsService.FrameRate.Value}\n" +
                        $"Scale: {_gameSettingsService.ScreenScale.Value}";
        }
        
        [Button]
        public void Apply()
        {
            if(_gameSettingsService == null)
                return;
            
            var xValue = int.TryParse(x.text, out var xResolution) ? xResolution : resolution.width;
            var yValue = int.TryParse(y.text, out var yResolution) ? yResolution : resolution.height;
            
            _gameSettingsService.SetScreenResolution(xValue, yValue);
            
            _gameSettingsService.DpiScale.Value = float.TryParse(dpi.text, out var dpiValue) 
                ? dpiValue :
                _gameSettingsService.DpiScale.Value;
            
            _gameSettingsService.FrameRate.Value = int.TryParse(fps.text, out var fpsValue) 
                ? fpsValue :
                _gameSettingsService.FrameRate.Value;
            
            _gameSettingsService.ScreenScale.Value = float.TryParse(scale.text, out var scaleValue) 
                ? scaleValue :
                _gameSettingsService.ScreenScale.Value;
            
            UpdateInfo();
        }

    }
}
