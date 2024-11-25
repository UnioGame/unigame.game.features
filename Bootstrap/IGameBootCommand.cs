namespace Vampire.Game.Modules.tma.features.Bootstrap
{
    using System;
    using Cysharp.Threading.Tasks;
    using UniGame.Core.Runtime;

    public interface IGameBootCommand
    {
        public UniTask<BootStepResult> ExecuteAsync(IContext context);
    }
    
    [Serializable]
    public struct BootStepResult
    {
        public bool success;
        public bool canContinue;
        public string error;
    }
}