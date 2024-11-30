namespace Game.Runtime.Services.DebugService
{
    using TMPro;
    using UnityEngine;
    
    public class ProductVersion : MonoBehaviour
    {
        public TextMeshProUGUI label; 
        private void Awake()
        {
            label.text = "v=" + Application.version;
        }
    }
}