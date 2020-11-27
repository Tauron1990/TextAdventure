using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using TextAdventures.Builder;
using TextAdventures.Engine.Actors;
using TextAdventures.Engine.CommandSystem;

namespace TextAdventures.Engine.Data
{
    public sealed class GameObjectManagerActor : GameProcess
    {
        private readonly Dictionary<string, GameObject> _gameObjects = new();

        private readonly Dictionary<Type, CommandProcessorBase> _commandProcessors = new();

        public GameObjectManagerActor()
        {
            Receive<GameSetup>(SetupGame);
            Receive<RegisterCommandProcessorBase>(RegisterCommandProcessor);
            Receive<IGameCommand>(RunCommand);
        }

        private void RunCommand(IGameCommand command)
        {
            if(!_commandProcessors.TryGetValue(command.GetType(), out var processor)) return;

            var game = Context.System.GetExtension<GameCore>();

            IEnumerable<(GameObject obj, object componet)> GetComponents(Type targetType)
            {
                if (string.IsNullOrWhiteSpace(command.Target))
                    foreach (var gameObject in _gameObjects.Select(p => p.Value).Where(gameObject => gameObject.HasComponent(targetType)))
                        yield return (gameObject, gameObject.GetComponent(targetType));
                else if (_gameObjects.TryGetValue(command.Target, out var gameObject) && gameObject.HasComponent(targetType))
                    yield return (gameObject, gameObject.GetComponent(targetType));
            }

            foreach (var (gameObject, componet) in GetComponents(processor.Component)) 
                processor.Run(game, componet, gameObject, command);
        }

        private void RegisterCommandProcessor(RegisterCommandProcessorBase message) 
            => _commandProcessors.Add(message.CommandType, message.CreateProcessor());

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