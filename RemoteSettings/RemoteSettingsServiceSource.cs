namespace Game.GameSettings
{
    using Cysharp.Threading.Tasks;
    using RemoteSettings;
    using Runtime;
    using UniGame.Context.Runtime;
    using UniGame.Core.Runtime;
    using UnityEngine;

    /// <summary>
    /// Game Settings Service Source
    /// </summary>
    [CreateAssetMenu(menuName = "Game/Services/Remote Settings/Remote Settings Service Source", 
        fileName = "Remote Settings Service Source")]
    public class RemoteSettingsServiceSource : DataSourceAsset<IRemoteSettingsService>
    {
        public bool useRemoteSettings = true;
        public RemoteSettingsLoader loader = new();
        
        protected override async UniTask<IRemoteSettingsService> CreateInternalAsync(IContext context)
        {
            if (useRemoteSettings)
                await LoadRemoteSettingsAsync(context);

            var dataAsset = RemoteModelAsset.ModelAsset;
            var gameModel = dataAsset.data;
            context.Publish<RemoteModelAsset>(dataAsset);
            context.Publish<IRemoteModel>(gameModel);
            return new RemoteSettingsService(gameModel);
        }
        
        public async UniTask<bool> LoadRemoteSettingsAsync(IContext context)
        {
            var result  = await loader.LoadRemoteSettings();
            return result;
        }
    }
}