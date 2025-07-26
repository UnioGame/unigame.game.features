namespace RemoteSettings
{
    using System;
    using Cysharp.Threading.Tasks;
    using Game.GameSettings;
    using Game.Runtime.Tools;
    using UniCore.Runtime.ProfilerTools;

    [Serializable]
    public class RemoteSettingsLoader
    {
        public async UniTask<bool> LoadRemoteSettings()
        {
            var fileName = RemoteModelAsset.RemoteSettingsName;
            try
            {
                var loadData = await 
                    StreamingAssetsUtils.LoadDataFromWeb(fileName);

                if (!loadData.success) return false;
                
                RemoteModelAsset.Reset();
                RemoteModelAsset.ModelAsset.ParseData(loadData.data);

                return true;
            }
            catch (Exception ex)
            {
                GameLog.LogError(ex);
                return false;
            }
        }
    }
}