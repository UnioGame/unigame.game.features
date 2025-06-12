namespace Game.Code.Services.GameSettingsService
{
    using R3;
    using Services.GameSettings.Data;
    using UniGame.GameFlow.Runtime;
    using UniGame.Runtime.Rx;

    public interface IGameSettingsService : IGameService
    {
        ScreenResolution DefaultResolution { get; }
        ScreenResolution ActiveResolution  { get; }
        
        ReactiveValue<bool> Profiler { get; }
        ReactiveValue<bool> Console { get; }
        ReactiveValue<int> FrameRate { get; }
        ReactiveValue<float> ScreenScale { get; }
        ReactiveValue<float> DpiScale { get; }

        void SetScreenResolution(ref ScreenResolution screenResolution, bool setRefreshRate = false);

        void SetScreenResolution(int width, int height);
    }
}