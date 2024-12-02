namespace Game.Runtime.Services.DebugService
{
    using System;
    using UnityEngine.AddressableAssets;

    [Serializable]
    public class DebugConfiguration
    {
        public AssetReferenceGameObject profilerPrefab;
        public AssetReferenceGameObject consolePrefab;
        public AssetReferenceGameObject versionPanelPrefab;
    }
}