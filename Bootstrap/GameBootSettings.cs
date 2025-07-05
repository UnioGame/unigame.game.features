namespace Game.Runtime.Bootstrap
{
    using System;
    using System.Collections.Generic;
    using Sirenix.OdinInspector;
    using UniGame.Context.Runtime;
    using UnityEngine;
    using Vampire.Game.Modules.tma.features.Bootstrap;

#if UNITY_EDITOR
    using UniModules.Editor;
#endif

    [CreateAssetMenu(menuName = "UniGame/Features/Game Boot Settings", fileName = nameof(GameBootSettings))]
    public class GameBootSettings : ScriptableObject
    {
        [Header("Service Sources")]
        public AsyncDataSources[] dataSources = Array.Empty<AsyncDataSources>();
        
        [InlineProperty]
        [HideLabel]
        public AsyncContextSource defaultSource = new();
        
        [Header("Commands")]
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