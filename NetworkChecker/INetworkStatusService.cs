namespace VN.Game.Runtime.Services
{
    using System;
    using Cysharp.Threading.Tasks;
    using UniGame.GameFlow.Runtime.Interfaces;

    public interface INetworkStatusService : IGameService
    {
        IObservable<NetworkCheckResult> InternetCheckStream { get; }

        UniTask FireNoInternetNotification(string error);
        UniTask<NetworkCheckResult> CheckInternet(string url);
        UniTask<NetworkCheckResult> CheckInternet();
    }
}