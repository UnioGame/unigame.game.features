namespace Game.Code.Services.GameSettingsService
{
    using UniGame.GameFlow.Runtime.Interfaces;
    using UniRx;

    public interface IGameSettingsService : IGameService
    {
        IReactiveProperty<bool> Profiler { get; }
        IReactiveProperty<bool> Console { get; }
        IReactiveProperty<int> FrameRate { get; }
        IReactiveProperty<float> ScreenScale { get; }
        IReactiveProperty<float> DpiScale { get; }
    }
}