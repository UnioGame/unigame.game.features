namespace Game.Modules.tma.features.GameImmortals
{
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using UniGame.Context.Runtime;
    using UniGame.Core.Runtime;
    using UnityEngine;
    using UnityEngine.AddressableAssets;

    [CreateAssetMenu(menuName = "Game/Services/GameImmortalObjectsService",
        fileName = "GameImmortalObjectsService")]
    public class GameImmortalObjectsServiceSource : DataSourceAsset<IGameImmortalObjectsService>
    {
        public List<AssetReferenceGameObject> immortalObjects = new();
        
        protected override async UniTask<IGameImmortalObjectsService> CreateInternalAsync(IContext context)
        {
            var service = new GameImmortalObjectsService(immortalObjects);
            await service.LoadAsync();
            return service;
        }
    }
    
}