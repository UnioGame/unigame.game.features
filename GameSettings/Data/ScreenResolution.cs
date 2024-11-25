namespace Game.Code.Services.GameSettings.Data
{
    using System;
    using UnityEngine;

    [Serializable]
    public struct ScreenResolution
    {
        public int width;
        public int height;
        public float aspect;
        public float dpi;
        public RefreshRate refreshRate;

        public override string ToString()
        {
            return $"w:{width} h:{height} aspect:{aspect} dpi:{dpi} refreshRate:{refreshRate}";
        }
    }
}