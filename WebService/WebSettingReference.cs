namespace Game.Runtime.Services.WebService
{
    using System;
    using UniGame.AddressableTools.Runtime.AssetReferencies;

    [Serializable]
    public class WebSettingReference : AddressableValue<WebSettings>
    {
        public string settingsId = string.Empty;
    }
}