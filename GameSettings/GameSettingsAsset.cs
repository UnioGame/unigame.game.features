namespace Game.Code.Services.GameSettingsService
{
    using Sirenix.OdinInspector;
    using UnityEngine;

    [CreateAssetMenu(menuName = "Game/Services/Settings/GameSettings Data", fileName = "GameSettings Data")]
    public class GameSettingsAsset : ScriptableObject
    {
        [InlineProperty]
        [HideLabel]
        public GameSettings settings = new GameSettings();
    }
}