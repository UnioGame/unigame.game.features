namespace Game.Runtime
{
    using System;
    using Newtonsoft.Json;

    [Serializable]
    public class RemoteGameModel : IRemoteModel
    {
        [JsonProperty("GameDataUrl")]
        public string gameDataUrl;
        [JsonIgnore]
        public string remoteFolder = "serverdata";
        [JsonIgnore]
        public bool addGameVersion = true;
        [JsonIgnore]
        public bool addBuildTarget = false;

        [JsonIgnore]
        public static bool IsDebug
        {
            get
            {
#if GAME_DEBUG
                return true;
#endif
                return false;
            }
        }

    }
}