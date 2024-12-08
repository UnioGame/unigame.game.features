namespace Game.Runtime.Services.WebService
{
    using System.Runtime.InteropServices;

    public static class UnityJSBridge
    {
        [DllImport("__Internal")]
        public static extern void RequestToken(string url);
        
        [DllImport("__Internal")]
        public static extern void Invite(string inviteUrl);
        
        [DllImport("__Internal")]
        public static extern void CopyLink(string link);

        [DllImport("__Internal")]
        public static extern void AdsgramShow(string type);
        
        [DllImport("__Internal")]
        public static extern void TonTonEvent(string screenNameUtf, string userTgIdUtf, string userRefUtf, string userUtmUtf);

        [DllImport("__Internal")]
        public static extern void OpenURL(string url);

        [DllImport("__Internal")]
        public static extern void RequestBoosterPayment(string link, string token);
    }
    
    
}