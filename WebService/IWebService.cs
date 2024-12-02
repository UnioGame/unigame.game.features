namespace Game.Runtime.Services.WebService
{
    using Cysharp.Threading.Tasks;
    using UniGame.GameFlow.Runtime.Interfaces;
    using UnityEngine;

    public interface IWebService : IGameService
    {
        WebMessageReceiver WebMessageReceiver { get; }
        
        string GetServerUrl(string path);
        
        void Authenticate();
        
        UniTask<Sprite> GetSpriteAsync(string avatarURL);
        
    }
}