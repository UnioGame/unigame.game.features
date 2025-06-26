﻿namespace Game.GameSettings
{
    using System;
    using Runtime;
    using UniGame.GameFlow.Runtime;

    [Serializable]
    public class RemoteSettingsService : GameService, IRemoteSettingsService
    {
        private RemoteGameModel _remoteGameModel;

        public RemoteSettingsService(RemoteGameModel remoteGameModel)
        {
            _remoteGameModel = remoteGameModel;
        }
    }
}