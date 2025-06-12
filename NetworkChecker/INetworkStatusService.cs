namespace VN.Game.Runtime.Services
{
    using System;
    using Cysharp.Threading.Tasks;
    using R3;
    using UniGame.GameFlow.Runtime;

    public interface INetworkStatusService : IGameService
    {
        Observable<NetworkCheckResult> InternetCheckStream { get; }
        UniTask FireNoInternetNotification(string error);
        UniTask<NetworkCheckResult> CheckInternet(string url);
        UniTask<NetworkCheckResult> CheckInternet();
    }
}