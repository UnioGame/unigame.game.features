namespace Game.Runtime.Bootstrap
{
    using System.Collections.Generic;
    using Sirenix.OdinInspector;
    using UniGame.Context.Runtime;
    using UnityEngine;
    using UniGame.Features.Bootstrap;

#if UNITY_EDITOR
    using UniModules.Editor;
#endif

    [CreateAssetMenu(menuName = "UniGame/Features/Game Boot Settings", fileName = nameof(GameBootSettings))]
    public class GameBootSettings : ScriptableObject
    {
        [Header("Service Sources")]
        [InlineProperty]
        [HideLabel]
        public AsyncContextSource source = new();
        
        [Header("Commands")]
        [SerializeReference]
        [Tooltip("Commands to execute before game initialization, e.g. loading assets, initializing services, etc.")]
        public List<IGameBootCommand> gameInitCommands = new();

#if UNITY_EDITOR
        [Button]
        public void Save()
        {
            this.SaveAsset();
        }
#endif
    }
}