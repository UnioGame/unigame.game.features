namespace Game.Code.Services.GameSettingsService
{
    
    using UnityEngine;
    
#if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
#endif

    [CreateAssetMenu(menuName = "Game/Services/Settings/GameSettings Data", fileName = "GameSettings Data")]
    public class GameSettingsAsset : ScriptableObject
    {
#if ODIN_INSPECTOR
        [InlineProperty]
        [HideLabel]
#endif
        public GameSettings settings = new GameSettings();
    }
}