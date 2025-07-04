namespace Game.Runtime.Bootstrap
{
    using System.Collections.Generic;
    using Sirenix.OdinInspector;
    using UniGame.Context.Runtime;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using Vampire.Game.Modules.tma.features.Bootstrap;

#if UNITY_EDITOR
    using UniModules.Editor;
#endif

    [CreateAssetMenu(menuName = "UniGame/Features/Game Boot Settings", fileName = nameof(GameBootSettings))]
    public class GameBootSettings : ScriptableObject
    {
        public bool bootSceneEnabled = true;
        
        [ShowIf(nameof(bootScene))]
        public AssetReference bootScene;
        
        public AsyncContextSource sources = new();
        
        [SerializeReference]
        public List<IGameBootCommand> bootCommands = new();

#if UNITY_EDITOR
        [Button]
        public void Save()
        {
            this.SaveAsset();
        }
#endif
    }
}