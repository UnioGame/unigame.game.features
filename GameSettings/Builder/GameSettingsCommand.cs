namespace Game.Code.Services.GameSettingsService.Builder
{
    using System;
    using UniGame.UniBuild.Editor.ClientBuild.Interfaces;
    using UniModules.Editor;
    using UniModules.UniGame.UniBuild.Editor.ClientBuild.Commands.PreBuildCommands;
    using UniModules.UniGame.UniBuild.Editor.ClientBuild.Interfaces;

    [Serializable]
    public class GameSettingsCommand : UnitySerializablePreBuildCommand
    {
        public bool console = true;
        public bool profiler = true;
        public int targetFrameRate = 30;

        public string consoleArgument = "-gameConsole";
        public string profilerArgument = "-gameProfiler";
        public string targetFrameRateArgument = "-targetFrameRate";
        
        public override void Execute(IUniBuilderConfiguration buildParameters)
        {
            var arguments = buildParameters.Arguments;
            var settingsAssets = AssetEditorTools.GetAssets<GameSettingsAsset>();
            
            var useConsole = arguments.Contains(consoleArgument) || console;
            var useProfiler = arguments.Contains(profilerArgument) || profiler;
            
            arguments.GetIntValue(targetFrameRateArgument, out var frameRate, targetFrameRate);
            
            foreach (var gameSettingsAsset in settingsAssets)
            {
                var settings = gameSettingsAsset.settings;
                settings.console = useConsole;
                settings.profiler = useProfiler;
                settings.targetFrameRate = frameRate;
                
                gameSettingsAsset.SaveAsset();
            }
        }
        
    }
}