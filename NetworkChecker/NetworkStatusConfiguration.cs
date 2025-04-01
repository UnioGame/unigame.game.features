namespace VN.Game.Runtime.Services
{
    using System;

    [Serializable]
    public class NetworkStatusConfiguration
    {
        public bool autoCheckInternet = true;
        public float autoCheckInterval = 3f;
        public string internetCheckUrl = "https://www.google.com";
    }
}