namespace Game.Runtime.Services.WebService
{
    using Cysharp.Threading.Tasks;
    using UniCore.Runtime.ProfilerTools;
    using UniGame.Core.Runtime.Rx;
    using UniModules.UniGame.Core.Runtime.Rx;
    using UnityEngine;

    public class WebMessageReceiver : MonoBehaviour, IWebMessageReceiver
    {
        #region inspector
        
        public double timeoutSec = 60f;
        public string boosterPurchaseSuccess = "paid";
        public string boosterPurchaseFailed = "failed";
        public ReactiveValue<bool> initialized = new();
        public ReactiveValue<string> tokenValue = new();
        
        #endregion
        
        public IReadonlyReactiveValue<string> Token => tokenValue;
        
        public async UniTask InitializeAsync(float timeout)
        {
            timeoutSec = timeout;
        }
        
        private void Awake()
        {
            gameObject.name = nameof(WebMessageReceiver);
            DontDestroyOnLoad(gameObject);
        }
    }
}