using System;
using System.Collections.Generic;
using TextAdventures.Builder;
using TextAdventures.Engine.Actors;

namespace TextAdventures.Engine.Data
{
    public sealed class GameObjectManager : GameProcess
    {
        private readonly Dictionary<string, GameObject> _gameObjects = new();
        
        public GameObjectManager()
        {
            Receive<GameSetup>(SetupGame);
        }

        private void SetupGame(GameSetup obj)
        {
            try
            {
                foreach (var blueprint in obj.GameObjectBlueprints)
                {
                    
                }
            }
            catch (Exception e)
            {
                obj.Error(e);
            }
        }
    }
}