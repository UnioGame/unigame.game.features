namespace Game.Runtime.Services.DebugService
{
    using System;
    using Cysharp.Threading.Tasks;
    using UniGame.AddressableTools.Runtime;
    using UniGame.UniNodes.GameFlow.Runtime;
    using UnityEngine;
    using Object = UnityEngine.Object;

    [Serializable]
    public class DebugService : GameService, IDebugService
    {
        private DebugConfiguration _debugConfiguration;
        private DebugItemData _console;
        private DebugItemData _profiler;
        private DebugItemData _version;
        
        public DebugService(DebugConfiguration debugConfiguration)
        {
            _debugConfiguration  = debugConfiguration;
            _console = new DebugItemData()
            {
                asset = null,
                source = _debugConfiguration.consolePrefab,
                state = false
            };
            _profiler = new DebugItemData()
            {
                asset = null,
                source = _debugConfiguration.profilerPrefab,
                state = false
            };
            _version = new DebugItemData()
            {
                asset = null,
                source = _debugConfiguration.versionPanelPrefab,
                state = false
            };
        }
        
        public async UniTask<DebugItemData> SetState(DebugItemData item, bool state)
        {
            if (!item.source.RuntimeKeyIsValid())
                return item;
            
            if(item.state == state) return item;

            if (!state)
            {
                item.state = false;
                if (item.asset == null) return item;
                Object.Destroy(item.asset);
                item.asset = null;
                return item;
            }
            
            item.state = true;
            var reference = item.source;
            var sourceAsset = await reference.LoadAssetTaskAsync<GameObject>(LifeTime);
            var instance = Object.Instantiate(sourceAsset);
            Object.DontDestroyOnLoad(instance);
            instance.transform.localScale = Vector3.one;
            instance.SetActive(true);
                
            item.asset = instance;
            return item;
        }

        public void SetVersionState(bool state)
        {
            SetState(_version, state).Forget();
        }
        
        public void SetProfilerState(bool state)
        {
            SetState(_profiler, state).Forget();
        }

        public void SetConsoleState(bool state)
        {
            SetState(_console, state).Forget();
        }
    }
}