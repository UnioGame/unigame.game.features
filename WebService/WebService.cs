namespace Game.Runtime.Services.WebService
{
    using System;
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using global::Modules.WebServer;
    using global::Modules.WebTexture;
    using UniGame.MetaBackend.Shared;
    using UniCore.Runtime.ProfilerTools;
    using UniGame.UniNodes.GameFlow.Runtime;
    using UniRx;
    using UnityEngine;
    using UnityEngine.Networking;

    public class WebService : GameService, IWebService
    {
        public const string AdsGramResult = "HandleAdsgramResult";
        public const string AdsGramFakeResult = "{\"error\":false,\"state\":\"load\",\"done\":true,\"description\":\"this is fake raw result\"}";

        private IBackendMetaService _metaService;
        private IWebMetaProvider _webMetaProvider;
        private readonly IWebTextureProvider _textureProvider;
        private WebSettings _settings;
        private bool _offline;
        private WebMessageReceiver _messageReceiver;
        private string _userToken = string.Empty;
        private Dictionary<string, string> _urlsCache;
        
        public WebService(
            IBackendMetaService metaService, 
            IWebMetaProvider webMetaProvider,
            IWebTextureProvider textureProvider,
            WebSettings settings, 
            bool offline, 
            WebMessageReceiver messageReceiver)
        {
            _urlsCache = new(64);
            _metaService = metaService;
            _webMetaProvider = webMetaProvider;
            _textureProvider = textureProvider;
            _settings = settings;
            _offline = offline;

#if UNITY_EDITOR
            _userToken = FakeToken;
            _webMetaProvider.SetToken(_userToken);
            _textureProvider.SetToken(_userToken);
#endif
            
            _messageReceiver = messageReceiver;
            _messageReceiver.Token
                .Subscribe(SetUserToken)
                .AddTo(LifeTime);
        }
        
        public string ServerUrl => _settings.apiURL;
        public string UserToken => _userToken;
        public bool FakeAuthEnabled => _settings.enableFakeAuthentication;
        public string FakeToken => $"{_settings.cheat}:{_settings.fakeUserTelegramId}".TrimEnd();
        
        
        public WebMessageReceiver WebMessageReceiver => _messageReceiver;
        
        public string GenerateSignUpUrl(string uuid, string rewardCode)
        {
            if(string.IsNullOrEmpty(rewardCode)) throw new ArgumentNullException("rewardCode");
            // # timestamp + "bastion" + "booster_ads" (тип Бустера) + user.uuid
            // # Такая строка должна получится для первой волны:
            //             1722874756449bastionbooster_ads4910b5d0-6508-4d8a-823c-2bc6d4df58bd
            // # Хешируем ее в md5 и получаем, например:
            //                 c0d64f977d7c987069114f73081e5325
            // # Используем эту строку как подпись
            var timestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
            var data = timestamp + "bastion" + rewardCode + uuid;
            var md5Provider = new System.Security.Cryptography.MD5CryptoServiceProvider();
            var bytes = System.Text.Encoding.UTF8.GetBytes(data);
            var hash = md5Provider.ComputeHash(bytes);
            var signUpUrl = BitConverter.ToString(hash).Replace("-", "").ToLower();
            GameLog.Log(data, Color.cyan);
            GameLog.Log(signUpUrl, Color.cyan);
            return signUpUrl;
        }

        public string GetServerUrl(string path)
        {
            if(_urlsCache.TryGetValue(path, out var url))
                return url;

            var server = ServerUrl;
            var uriBuilder = new UriBuilder(server)
            {
                Path = path
            };
            
            var result = uriBuilder.Uri.ToString();
            _urlsCache[path] = result;
            return result;
        }

        public async UniTask<WebRequestResult<TOut>> ExecuteAsync<TIn,TOut>(IRemoteMetaContract<TIn,TOut> contract) 
            where TOut : class
        {
            var result = await ExecuteAsync(contract as IRemoteMetaContract);
            
            var resultT = new WebRequestResult<TOut>()
            {
                success = result.success,
                data = result.data as TOut,
                error = result.error,
            };

            return resultT;
        }

        public async UniTask<WebRequestResult> ExecuteAsync(IRemoteMetaContract contract)
        {
            var result = new WebRequestResult()
            {
                success = false,
                data = default,
                error = string.Empty,
            };
            
            var requestResult = await _metaService.ExecuteAsync(contract);
            result.success = requestResult.success;
            result.data = requestResult.result;
            result.error = requestResult.error;
            return result;
        }

        public async UniTask<Texture2D> GetTexture(string url)
        {
            var request = UnityWebRequestTexture.GetTexture(url);
            try
            {
                 await request.SendWebRequest();
            }
            catch (Exception e)
            {
                Console.WriteLine("GetTexture web request exception: " + e);
                return null;
            }
            return DownloadHandlerTexture.GetContent(request);
        }

        public async UniTask<Sprite> GetSpriteAsync(string avatarURL)
        {
            if (_offline)
            {
                return Sprite.Create(new Texture2D(8, 8), new Rect(0, 0, 8, 8), Vector2.zero);
            }
            var finalUrl = ServerUrl + avatarURL;
            var result = await GetTexture(finalUrl);
            
            if (result == null) return null;

            return Sprite.Create(result, new Rect(0, 0, result.width, result.height), Vector2.zero);
        }
        
        /// <summary>
        /// ответ на запрос токена должен прийти через метод OnTokenReceived в WebMessageReceiver
        /// </summary>
        public void Authenticate()
        {
        }

        public void SetUserToken(string token)
        {
            _userToken = token;
            _webMetaProvider.SetToken(_userToken);
            _textureProvider.SetToken(_userToken);
            GameLog.Log($"User token set: {token}",Color.grey);
        }


        // https://t.me/ТВОЙБОТ?start=refПОЛЬЗОВАТЕЛЬUUID (https://t.me/share/url?url=https://t.me/%D0%A2%D0%92%D0%9E%D0%99%D0%91%D0%9E%D0%A2?start=ref%D0%9F%D0%9E%D0%9B%D0%AC%D0%97%D0%9E%D0%92%D0%90%D0%A2%D0%95%D0%9B%D0%ACUUID)
        //public string GetInviteUrl(string uuid) => $"https://t.me/{_settings.bot}?start=ref{uuid}";

    }
}