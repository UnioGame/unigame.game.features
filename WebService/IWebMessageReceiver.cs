namespace Game.Runtime.Services.WebService
{
    using UniGame.Core.Runtime.Rx;

    public interface IWebMessageReceiver
    {
        IReadonlyReactiveValue<string> Token { get; }
    }
}