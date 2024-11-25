namespace Game.Code.Services.GameSettingsService
{
    using Services.GameSettings.Data;
    using UniGame.GameFlow.Runtime.Interfaces;
    using UniRx;

    public interface IGameSettingsService : IGameService
    {
        ScreenResolution DefaultResolution { get; }
        ScreenResolution ActiveResolution  { get; }
        IReactiveProperty<bool> Profiler { get; }
        IReactiveProperty<bool> Console { get; }
        IReactiveProperty<int> FrameRate { get; }
        IReactiveProperty<float> ScreenScale { get; }
        IReactiveProperty<float> DpiScale { get; }

        void SetScreenResolution(ref ScreenResolution screenResolution, bool setRefreshRate = false);

        void SetScreenResolution(int width, int height);
    }
}