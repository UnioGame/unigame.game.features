namespace Game.Runtime.Bootstrap
{
    using System.Collections.Generic;
    using System.Linq;
    using Sirenix.OdinInspector;
    using UniGame.Context.Runtime;
    using UnityEngine;
    using UniGame.Features.Bootstrap;

#if UNITY_EDITOR

#if ODIN_INSPECTOR
    using Sirenix.Utilities.Editor;
#endif
    
    using UniModules.Editor;
#endif

    [CreateAssetMenu(menuName = "UniGame/Bootstrap/GameBootSettings", fileName = nameof(GameBootSettings))]
    public class GameBootSettings : ScriptableObject
    {
        [Header("Service Sources")]
        [InlineProperty]
        [HideLabel]
        public AsyncContextSource source = new();
        
        [Header("Commands")]
        [SerializeReference]
        [Tooltip("Commands to execute before game initialization, e.g. loading assets, initializing services, etc.")]
        [ListDrawerSettings(OnEndListElementGUI = nameof(EndDrawListElement))]
        public List<IGameBootCommand> gameInitCommands = new();

        
        private void EndDrawListElement(int index)
        {
#if UNITY_EDITOR
            if(gameInitCommands.Count <= index) return;
            SirenixEditorGUI.EndBox();
            if (!SirenixEditorGUI.Button("open", ButtonSizes.Medium)) return;
            var command = gameInitCommands[index];
            var type = command?.GetType();
            if (type == null) return;
            type.OpenEditorScript();
#endif
        }

#if UNITY_EDITOR
        
        [Button]
        public void Save()
        {
            this.SaveAsset();
        }

        [Button]
        public void Fill()
        {
            var sources = AssetEditorTools.GetAssets<DataSourceAsset>();
            var descriptions = source.asyncSources;
            descriptions.RemoveAll(x => x == null || x.source.editorAsset == null);
            
            var results = new List<AsyncSourceDescription>(descriptions);

            foreach (var sourceAsset in sources)
            {
                var foundSource = descriptions.FirstOrDefault(x => x.source.editorAsset == sourceAsset);
                if(foundSource!=null) continue;
                
                var newDescription = new AsyncSourceDescription
                {
                    source = new AssetReferenceDataSource(sourceAsset.GetGUID()),
                    enabled = true,
                    awaitLoading = false
                };
                
                results.Add(newDescription);
            }

            descriptions.Clear();
            descriptions.AddRange(results);
            
            this.MarkDirty();
        }
        
#endif
    }
}