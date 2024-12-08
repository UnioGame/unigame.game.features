namespace Game.Ecs.Authentication.Systems
{
    using System;
    using Runtime.Services.WebService;

    [Serializable]
    public class UserData
    {
        // "user": {
        //     "uuid": "c51af627-ecef-4ca9-abc0-3f30009ea69b",
        //     "telegram": 81051410,
        //     "connection": "Wallet",
        //     "wallet": "UQAThTeV8Y5TbQabOn-ltnV_gbxmW6Mpykk0tDZh3ZZKteJn",
        //     "name": "Ivan Ivanov",
        //     "image": "images/uploads/81051410_profile_photo.jpg",
        //     "balance": 100,
        //     "tutorial_completed": false,
        //     "created_at": "2024-06-15T19:08:30.316Z",
        //     "updated_at": "2024-06-19T10:00:30.100Z"
        // },
        // "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ0ZWxlZ3JhbSI6ODExNTIxNDI3LCJpYXQiOjE3MTk3NzgyODIsImV4cCI6MTcyMDEzODI4Mn0.Y6DXGTvhxPe6grXAqVDT7Vx5Eh_9fZ4uMvuYykEYoaM"
        public Data user;
        public string token;
    }
}