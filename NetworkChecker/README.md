# Network Checker

A simeple service to check internet connection status and notify the game when it changes

# Usage

Initialize NetworkCheckerService with NetworkStatusConfigurations


INetworkStatusService API:
 
```csharp
    public interface INetworkStatusService : IGameService
    {
        IObservable<NetworkCheckResult> InternetCheckStream { get; }
        UniTask FireNoInternetNotification(string error);
        UniTask<NetworkCheckResult> CheckInternet(string url);
        UniTask<NetworkCheckResult> CheckInternet();
    }
```


