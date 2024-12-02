namespace Game.Runtime.Services.WebService
{
    using System;

    [Serializable]
    public class Data
    {
        public string uuid;
        public uint telegram;
        public string connection;
        public string wallet;
        public string name;
        public string image;
        public int balance;
        public bool tutorial_completed;
        public DateTime created_at;
        public DateTime updated_at;
    }
}