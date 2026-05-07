namespace Game.Code.Services.GameSettingsService
{
    using Cysharp.Threading.Tasks;
    
    using UniGame.Context.Runtime;
    using UniGame.Core.Runtime;
    using UnityEngine;

#if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
#endif
    
#if UNITY_EDITOR
    using UnityEditor;
#endif

    /// <summary>
    /// game settings service
    /// </summary>
    [CreateAssetMenu(menuName = "Game/Services/Settings/GameSettings Service Source", fileName = "GameSettings Service Source")]
    public class GameSettingsServiceSource : DataSourceAsset<IGameSettingsService>
    {
#if ODIN_INSPECTOR
        [InlineEditor]
#endif
        public GameSettingsAsset settingsAsset;
        
        private GameSettingsService _service;
        
        protected override async UniTask<IGameSettingsService> CreateInternalAsync(IContext context)
        {
            var lifeTime = context.LifeTime;
            var settingsData = Instantiate(settingsAsset);
            var settings = settingsData.settings;

            _service = new GameSettingsService(settings);

#if UNITY_EDITOR
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
#endif
            
            return _service;
        }
        
#if UNITY_EDITOR
        private void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            switch (state)
            {
                case PlayModeStateChange.EnteredEditMode:
                case PlayModeStateChange.ExitingEditMode:
                    EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
                    _service?.Reset();
                    break;
            }
        }
        
#if ODIN_INSPECTOR
        [Button]
#endif
        public void ApplySettings()
        {
            var settingsData = Instantiate(settingsAsset);
            var settings = settingsData.settings;
            var service = new GameSettingsService(settings);
        }
#endif
    }
}