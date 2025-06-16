namespace VN.Game.Runtime.Services
{
    using System;
    using Cysharp.Threading.Tasks;
    using R3;
    using UniGame.UniNodes.GameFlow.Runtime;
    using UnityEngine;
    using UnityEngine.Networking;
    using ZLinq;

    [Serializable]
    public class InternetStatusService : GameService, INetworkStatusService
    {
        public const string ManualNoInternetError = "Manual: No internet connection";
        
        private NetworkStatusConfiguration _configuration;
        private Subject<NetworkCheckResult> _onInternetCheck;
        private bool _isAutoCheckInternetActive;

        public InternetStatusService(NetworkStatusConfiguration configuration)
        {
            _configuration = configuration;
            _onInternetCheck = new Subject<NetworkCheckResult>()
                .AddTo(LifeTime);
            
            if(configuration.autoCheckInternet)
                AutoCheckInternetLoop().Forget();
        }
        
        public Observable<NetworkCheckResult> InternetCheckStream => _onInternetCheck;

        public async UniTask FireNoInternetNotification(string error)
        {
            _onInternetCheck.OnNext(new NetworkCheckResult()
            {
                Duration = 0,
                IsSuccess = false,
                Error = error,
                Url = ManualNoInternetError,
            });
        }

        public async UniTask<NetworkCheckResult> CheckInternet(string url)
        {
            var startTime = Time.realtimeSinceStartup;
            var checkResult = new NetworkCheckResult();
            try
            {
                var result = await UnityWebRequest
                    .Get(url)
                    .SendWebRequest()
                    .ToUniTask()
                    .SuppressCancellationThrow();

                var duration = Time.realtimeSinceStartup - startTime;
                var isCanceled = result.IsCanceled;
                var requestResult = result.Result;
                var isSuccess = !isCanceled &&
                                  requestResult.result == UnityWebRequest.Result.Success;

                checkResult = new NetworkCheckResult()
                {
                    Duration = duration,
                    Error = requestResult.error,
                    Url = url,
                    IsSuccess = isSuccess,
                };

                if (!isSuccess)
                {
                    var message =
                        $"{nameof(InternetStatusService)}: CheckInternet | Something went wrong with internet: {requestResult.error}";
                    Debug.LogError(message);
                }
            }
            catch (Exception ex)
            {
                checkResult.Error = ex.Message;
                checkResult.IsSuccess = false;
                checkResult.Url = url;
                checkResult.Duration = Time.realtimeSinceStartup - startTime;
            }
            
            _onInternetCheck.OnNext(checkResult);
            
            return checkResult;
        }

        public async UniTask<NetworkCheckResult> CheckInternet()
        {
            if (_configuration.internetCheckUrl.Length <= 0)
            {
                return new NetworkCheckResult()
                {
                    IsSuccess = true,
                    Url = string.Empty,
                    Duration = 0,
                    Error = string.Empty,
                };
            }
            
            var checkTasks = _configuration
                .internetCheckUrl
                .Select(CheckInternet);

            var results = await UniTask.WhenAll(checkTasks);
            var failedCount = 0;
            var successCount = 0;
            
            var lastFailed = new NetworkCheckResult();
            var lastSuccess = new NetworkCheckResult()
            {
                IsSuccess = false,
                Url = string.Empty,
                Duration = 0,
                Error = string.Empty,
            };
            
            for (var i = 0; i < results.Length; i++)
            {
                var checkResult = results[i];

                if (checkResult.IsSuccess)
                {
                    successCount++;
                }
                else
                {
                    failedCount++;
                }

                lastFailed = checkResult.IsSuccess == false ? checkResult : lastFailed;
                lastSuccess = checkResult.IsSuccess ? checkResult : lastSuccess;
            }

            if (failedCount == results.Length)
                return lastFailed;
            
            return lastSuccess;
        }
        
        
        private async UniTask AutoCheckInternetLoop()
        {
            if(_isAutoCheckInternetActive) return;
            
            while (LifeTime.IsTerminated == false)
            {
                await CheckInternet();
                await UniTask.Delay(TimeSpan.FromSeconds(_configuration.autoCheckInterval));
            }
            
            _isAutoCheckInternetActive = false;
        }
    }
}