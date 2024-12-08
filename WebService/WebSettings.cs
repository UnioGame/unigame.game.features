namespace Game.Runtime.Services.WebService
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Sirenix.OdinInspector;
    using UnityEngine;

    [Serializable]
    [CreateAssetMenu(menuName = "Game/Services/WebRequest Settings",
        fileName = "WebRequest Settings")]
    public class WebSettings : ScriptableObject
    {
        public static WebSettings Default(bool generateTestSettings = false) => CreateInstance<WebSettings>();
        
        public string userToken = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJPbmxpbmUgSldUIEJ1aWxkZXIiLCJpYXQiOjE3MTgxMzc4MDcsImV4cCI6MTc0OTY3MzgwNywiYXVkIjoid3d3LmV4YW1wbGUuY29tIiwic3ViIjoianJvY2tldEBleGFtcGxlLmNvbSIsIkdpdmVuTmFtZSI6IkpvaG5ueSIsIlN1cm5hbWUiOiJSb2NrZXQiLCJFbWFpbCI6Impyb2NrZXRAZXhhbXBsZS5jb20iLCJSb2xlIjpbIk1hbmFnZXIiLCJQcm9qZWN0IEFkbWluaXN0cmF0b3IiXX0.lJVUJfNgsbp7Lw3Jl_DI40j5Zj5P4GNfz-Ln2TgQivE";
        public string apiURL = "http://localhost:8080";
        public int timeout = 30;

        [TitleGroup("Bot")]
        public string bot = "BastionBattle_bot";
        //todo парсить из swagger 
        [TitleGroup("Swagger API")]
        public string getProfile = "/api/profile";
        [TitleGroup("Swagger API")]
        public string getNotifications = "/api/profile/notifications";
        [TitleGroup("Swagger API")]
        public string getAvatar = "/api/profile/avatar";
        
        [TitleGroup("Swagger API")]
        public string updateProfile = "/api/profile";
        [TitleGroup("Swagger API")]
        public string auth = "/api/auth";
        
        [Space(10)]
        [TitleGroup("Rewards")]
        public string getRewards = "/api/profile/rewards";

        [TitleGroup("Rewards")]
        public string createReward = "/api/game/create_reward";
        
        [TitleGroup("Rewards")]
        public string getReferrals = "/api/profile/referals";
        [TitleGroup("Rewards")]
        public string createFinishWaveReward = "/api/profile/rewards/request";
        [TitleGroup("Rewards")]
        public string claimRewards = "/api/profile/rewards/claim";
        [Space(10)]
        [TitleGroup("Progress")]
        public string getProgress = "/api/profile/progress";
        [Space(10)]
        [TitleGroup("Tasks")]
        public string getTasks = "/api/game/tasks";
        [TitleGroup("Tasks")]
        public string openTask = "/api/profile/tasks/complete";
        
        [Space(10)]
        public bool enableFakeAuthentication = false;
        [EnableIf("enableFakeAuthentication")]
        public string cheat = "UY2G1XPga3hkxPkK6RMbqXwRyDX6hCZSqMStGj39n5WnmZMvU5zupj5KC6T4";
        
        [EnableIf("enableFakeAuthentication")]
        public string fakeUserTelegramId;

        [TitleGroup("Boosters")]
        public string getBooster = "/api/profile/boosters";
        [TitleGroup("Boosters")]
        public string getBoosterActive = "/api/profile/boosters/active";
        [TitleGroup("Boosters")]
        public string createBooster = "/api/profile/boosters/request";

        [TitleGroup("Collection")]
        public string getCharacters = "/api/profile/characters";
        [TitleGroup("Collection")]
        public string upgradeCard = "/api/profile/characters/?/upgrade";

        [TitleGroup("Rewards/Daily Bonus")]
        public string getDailyBonus = "/api/profile/daily";
        [TitleGroup("Rewards/Daily Bonus")]
        public string collectDailyBonus = "/api/profile/daily/claim";

        [TitleGroup("Rewards/Daily Bonus")]
        public string claimDailyBonus = "/api/profile/daily/claim";

        [Button]
        [PropertyOrder(-1)]
        public string SerializeToJson()
        {
            var result = JsonConvert.SerializeObject(new WebSettingsPublicData
            {
                apiURL = apiURL,
                timeoutInSeconds = timeout
            });
            
            Debug.Log(result);
#if UNITY_EDITOR
            GUIUtility.systemCopyBuffer = result;
#endif
            return result;
        }
        
    }
}