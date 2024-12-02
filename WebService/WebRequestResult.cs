namespace Game.Runtime.Services.WebService
{
    public struct WebRequestResult
    {
        public bool success;
        public object data;
        public string url;
        public string error;
    }
    
    public struct WebRequestResult<T>
    {
        public bool success;
        public string url;
        public T data;
        public string error;
    }
    
}