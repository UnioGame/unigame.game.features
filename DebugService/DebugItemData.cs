namespace Game.Runtime.Services.DebugService
{
    using System;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using UnityEngine.Serialization;

    [Serializable]
    public class DebugItemData
    {
        public GameObject asset;
        public AssetReferenceGameObject source;
        [FormerlySerializedAs("active")]
        public bool state;
    }
}