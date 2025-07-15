namespace Bootstrap
{
    using System;
    using Cysharp.Threading.Tasks;
    using UniGame.Core.Runtime;
    using UniGame.Features.Bootstrap;
    using UnityEngine;

    [Serializable]
    public class CheckAndClearBundleCacheByVersionCommand : IGameBootCommand
    {
        public async UniTask<BootStepResult> ExecuteAsync(IContext context)
        {
            var result = new BootStepResult
            {
                success = true,
                canContinue = true,
                error = string.Empty,
            };
            
            var version = PlayerPrefs.GetString(nameof(Application.version), string.Empty);
            if (string.Equals(version, Application.version)) return result;

            PlayerPrefs.SetString(nameof(Application.version), Application.version);

#if !UNITY_WEBGL
            Caching.ClearCache();
#endif

            return result;
        }
    }
}