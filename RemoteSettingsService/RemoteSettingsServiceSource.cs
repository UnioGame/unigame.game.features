namespace Game.GameSettings
{
    using Cysharp.Threading.Tasks;
    using Runtime;
    using UniGame.Core.Runtime;
    using UniGame.GameFlow.Runtime.Services;
    using UnityEngine;

    /// <summary>
    /// Game Settings Service Source
    /// </summary>
    [CreateAssetMenu(menuName = "Game/Services/GameSettings/Remote Settings Service Source", 
        fileName = "Remote Settings Service Source")]
    public class RemoteSettingsServiceSource : DataSourceAsset<IRemoteSettingsService>
    {
        protected override async UniTask<IRemoteSettingsService> CreateInternalAsync(IContext context)
        {
            var dataAsset = Resources.Load<RemoteModelAsset>(nameof(RemoteModelAsset));
            var asset = Instantiate(dataAsset);
            var gameModel = asset.data;
            context.Publish<IRemoteModel>(gameModel);
            return new RemoteSettingsService(gameModel);
        }
    }
}