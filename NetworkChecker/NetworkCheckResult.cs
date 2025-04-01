namespace VN.Game.Runtime.Services
{
    using System;

    [Serializable]
    public struct NetworkCheckResult
    {
        public string Url;
        public bool IsSuccess;
        public float Duration;
        public string Error;
    }
}