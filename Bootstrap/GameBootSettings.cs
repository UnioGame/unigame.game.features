namespace Game.Runtime.Bootstrap
{
    
    using System.Collections.Generic;
    using System.Linq;
    using UniGame.Context.Runtime;
    using UnityEngine;
    using UniGame.Features.Bootstrap;

#if UNITY_EDITOR
    using UniGame.AddressableTools.Editor;
    using UniModules.Editor;
    
#if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
    using Sirenix.Utilities.Editor;
#endif
    
#endif

    [CreateAssetMenu(menuName = "UniGame/Bootstrap/GameBootSettings", fileName = nameof(GameBootSettings))]
    public class GameBootSettings : ScriptableObject
    {
        [Header("Service Sources")]
#if UNITY_EDITOR && ODIN_INSPECTOR
        [InlineProperty]
        [HideLabel]
#endif
        public AsyncContextSource source = new();
        
        [Header("Commands")]
        [SerializeReference]
        [Tooltip("Commands to execute before game initialization, e.g. loading assets, initializing services, etc.")]
#if UNITY_EDITOR && ODIN_INSPECTOR
        [ListDrawerSettings(OnEndListElementGUI = nameof(EndDrawListElement))]
#endif
        public List<IGameBootCommand> gameInitCommands = new();
        
        private void EndDrawListElement(int index)
        {
#if UNITY_EDITOR && ODIN_INSPECTOR
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
        
#if ODIN_INSPECTOR
        [Button]
#endif
        public void Save()
        {
            this.SaveAsset();
        }

#if ODIN_INSPECTOR
        [Button]
#endif
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

                if (!sourceAsset.IsAddressableAsset())
                    sourceAsset.MakeAddressable();
                
                results.Add(newDescription);
            }

            descriptions.Clear();
            descriptions.AddRange(results);
            
            this.MarkDirty();
        }
        
#endif
    }
}