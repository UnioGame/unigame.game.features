namespace Game.Runtime.Services.DebugService
{
    using UniGame.GameFlow.Runtime.Interfaces;

    public interface IDebugService : IGameService
    {
        void SetProfilerState(bool state);
        void SetConsoleState(bool state);
        void SetVersionState(bool state);
    }
}