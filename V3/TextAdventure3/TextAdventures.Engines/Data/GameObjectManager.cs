using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
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

        private void SetupGame(GameSetup setup)
        {
            try
            {
                foreach (var blueprint in setup.GameObjectBlueprints)
                {
                    var obj = new GameObject(blueprint.Name,
                                             blueprint.ComponentBlueprints
                                                      .Select(b => new ComponentObject(
                                                                                       Activator.CreateInstance(b.ComponentType) ?? throw new InvalidOperationException($"Componet Could not Created {b.ComponentType}")))
                                                      .ToImmutableList());

                    _gameObjects.Add(blueprint.Name, obj);
                }
            }
            catch (Exception e)
            {
                setup.Error(e);
            }
            finally
            {
                Sender.Tell(setup);
            }
        }
    }
}