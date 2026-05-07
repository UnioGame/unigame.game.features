namespace Game.Runtime
{
    using System;
    using Newtonsoft.Json;
    using UnityEngine;

#if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
#endif
    
    [Serializable]
    public class RemoteGameModel : IRemoteModel
    {
        [Header("if true use related url based on Application.absoluteURL")]
        [JsonProperty("UseRelatedUrl")]
        public bool useRelatedUrl = true;
            
        [JsonProperty("GameDataUrl")]
        public string gameDataUrl;
        
        [JsonProperty("AddGameVersion")]
        public bool addGameVersion = true;
        
        [JsonProperty("AddBuildTarget")]
        public bool addBuildTarget = false;

        [JsonProperty("OverrideAddressableUrl")]
        public bool overrideAddressableUrl = false;
        
        [JsonProperty("AddressableUrl")]
#if ODIN_INSPECTOR
        [ShowIf(nameof(overrideAddressableUrl))]
#endif
        public string addressableUrl = string.Empty;
        
        [JsonProperty("RemoteDataPath")]
        public string addressablePath = "serverdata";
        
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