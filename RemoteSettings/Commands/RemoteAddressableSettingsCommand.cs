namespace RemoteSettings
{
    using System;
    using Cysharp.Threading.Tasks;
    using Game.GameSettings;
    using Game.Runtime.Tools;
    using UniGame.Core.Runtime;
    using UniGame.Features.Bootstrap;
    using UnityEngine;

    [Serializable]
    public class RemoteAddressableSettingsCommand : IGameBootCommand
    {
        public async UniTask<BootStepResult> ExecuteAsync(IContext context)
        {
            var result = new BootStepResult()
            {
                success = false,
                canContinue = false,
                error = string.Empty,
            };
            
            var fileName = RemoteModelAsset.RemoteSettingsName;
            try
            {
                var loadData = await 
                    StreamingAssetsUtils.LoadDataFromWeb(fileName);

                if (loadData.success)
                {
                    RemoteModelAsset.Reset();
                    RemoteModelAsset.ModelAsset.ParseData(loadData.data);
                }
                
                result.success = loadData.success;
                result.canContinue = result.success;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            
            return result;
        }
    }
}