namespace Game.Modules.tma.features.GameImmortals
{
    using System;
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using UniGame.AddressableTools.Runtime;
    using UniGame.GameFlow.Runtime;
    using UniGame.UniNodes.GameFlow.Runtime;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using Object = UnityEngine.Object;

    [Serializable]
    public class GameImmortalObjectsService : GameService, IGameImmortalObjectsService
    {
        private readonly List<AssetReferenceGameObject> _immortalObjects;
        
        public List<GameObject> pawns = new();

        public GameImmortalObjectsService(List<AssetReferenceGameObject> immortalObjects)
        {
            _immortalObjects = immortalObjects;
        }
        
        public async UniTask LoadAsync()
        {
            var tasks = _immortalObjects
                .Select(x =>
                x.LoadAssetTaskAsync(LifeTime));
            
            var sources = await UniTask.WhenAll(tasks);

            foreach (var asset in sources)
            {
                if(asset == null) continue;
                var instance = UnityEngine.Object.Instantiate(asset);
                Object.DontDestroyOnLoad(instance);
                pawns.Add(instance);
            }
        }
        
    }

    public interface IGameImmortalObjectsService : IGameService
    {

    }


}