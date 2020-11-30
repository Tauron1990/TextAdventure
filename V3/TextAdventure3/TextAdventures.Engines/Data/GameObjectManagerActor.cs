using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Akka.Actor;
using TextAdventures.Builder;
using TextAdventures.Engine.Actors;
using TextAdventures.Engine.CommandSystem;
using TextAdventures.Engine.Storage;

namespace TextAdventures.Engine.Data
{
    public sealed class GameObjectManagerActor : GameProcess
    {
        private const string GlobalGameObject = "Game_Global_GameObject_Container";

        private readonly Dictionary<Type, CommandProcessorBase> _commandProcessors = new();
        private readonly Dictionary<string, GameObject> _gameObjects = new();

        public GameObjectManagerActor()
        {
            Receive<IGameCommand>(RunCommand);
            Receive<RegisterCommandProcessorBase>(RegisterCommandProcessor);

            Receive<RequestGameComponent>(r =>
                                          {
                                              if (_gameObjects.TryGetValue(GlobalGameObject, out var gameObject) && gameObject.HasComponent(r.Type))
                                              {
                                                  Sender.Tell(gameObject.GetComponent(r.Type));
                                                  return;
                                              }

                                              Sender.Tell(new RespondGameComponent(null));
                                          });
            Receive<RequestGameObject>(r =>
                                       {
                                           if (_gameObjects.TryGetValue(r.Name, out var gameObject))
                                           {
                                               Sender.Tell(new RespondGameObject(gameObject));
                                               return;
                                           }

                                           Sender.Tell(new RespondGameObject(null));
                                       });

            Receive<ComponentBlueprint>(UpdateGlobalObject);
            Receive<GameSetup>(SetupGame);

            Receive<FillData>(FillSaveData);
            Receive<UpdateData>(UpdateSaveData);
        }

        private void UpdateGlobalObject(ComponentBlueprint blueprint)
        {
            if (!_gameObjects.TryGetValue(GlobalGameObject, out var gameObject)) 
                gameObject = new GameObject(GlobalGameObject, ImmutableDictionary<Type, ComponentObject>.Empty);

            var comp = CreateComponent(blueprint);
            _gameObjects[GlobalGameObject] = gameObject with{Components = gameObject.Components.Add(comp.ComponentType, comp)};
        }

        private void UpdateSaveData(UpdateData updateData)
        {
            try
            {
                foreach (var data in updateData.Data)
                {
                    if (!_gameObjects.TryGetValue(data.Key, out var targetObject))
                        targetObject = new GameObject(data.Key, ImmutableDictionary<Type, ComponentObject>.Empty);

                    var componentData = targetObject.Components;

                    foreach (var componetDataValue in data.Value)
                    {
                        if (!componentData.TryGetValue(componetDataValue.Key, out var componentObject))
                        {
                            componentObject = new ComponentObject(
                                                                  Activator.CreateInstance(componetDataValue.Key)
                                                               ?? throw new InvalidOperationException($"Componet Could not Created {componetDataValue.Key}"));

                            componentData = componentData.Add(componentObject.ComponentType, componentObject);
                        }

                        if (componentObject.Component is ComponentBase componentBase)
                            componentBase.Cache = componetDataValue.Value;
                    }

                    _gameObjects[data.Key] = targetObject;
                }
            }
            finally
            {
                Sender.Tell(updateData);
            }
        }

        private void FillSaveData(FillData data)
        {
            ImmutableDictionary<string, object?> GetComponentData(ComponentObject comp)
            {
                if (comp.Component is ComponentBase componentBase)
                    return componentBase.Cache;

                return ImmutableDictionary<string, object?>.Empty;
            }

            ImmutableDictionary<Type, ImmutableDictionary<string, object?>> GetGameObjectData(GameObject obj) 
                => obj.Components.ToImmutableDictionary(c => c.Key, c => GetComponentData(c.Value));

            try
            {
                var newData = _gameObjects
                   .ToImmutableDictionary(e => e.Key,
                                          e => GetGameObjectData(e.Value));

                data = data with{Data = newData};
            }
            finally
            {
                Sender.Tell(data);
            }
        }

        private void RunCommand(IGameCommand command)
        {
            if (!_commandProcessors.TryGetValue(command.GetType(), out var processor)) return;

            var game = Context.System.GetExtension<GameCore>();

            IEnumerable<(GameObject obj, object componet)> GetComponents(Type targetType)
            {
                if (string.IsNullOrWhiteSpace(command.Target))
                {
                    foreach (var gameObject in _gameObjects.Select(p => p.Value).Where(gameObject => gameObject.HasComponent(targetType)))
                        yield return (gameObject, gameObject.GetComponent(targetType));
                }
                else if (_gameObjects.TryGetValue(command.Target, out var gameObject) && gameObject.HasComponent(targetType))
                    yield return (gameObject, gameObject.GetComponent(targetType));
            }

            foreach (var (gameObject, componet) in GetComponents(processor.Component).Where(c => processor.CanProcess(c.componet)))
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
                    var dic = blueprint.ComponentBlueprints
                                       .Select(p => (p.ComponentType, CreateComponent(p)))
                                       .ToImmutableDictionary(p => p.ComponentType, p => p.Item2);

                    _gameObjects.Add(blueprint.Name, new GameObject(blueprint.Name, dic));
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

        private static ComponentObject CreateComponent(ComponentBlueprint blueprint)
        {
            var inst = Activator.CreateInstance(blueprint.ComponentType);
            switch (inst)
            {
                case null:
                    throw new InvalidOperationException($"Componet Could not Created {blueprint.ComponentType}");
                case ComponentBase componentBase:
                    componentBase.Cache = blueprint.DefaultValues;
                    break;
            }

            return new ComponentObject(inst);
        }
    }
}