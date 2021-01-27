using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Linq;
using Akka.Actor;
using Tauron;
using Tauron.Features;
using TextAdventures.Builder;
using TextAdventures.Engine.Actors;
using TextAdventures.Engine.CommandSystem;
using TextAdventures.Engine.Storage;

namespace TextAdventures.Engine.Data
{
    public sealed class GameObjectManagerActor : GameProcess<GameObjectManagerActor.GomState>
    {
        public record GomState(ImmutableDictionary<Type, CommandProcessorBase> CommandProcessors, ImmutableDictionary<string, GameObject> GameObjects);

        public static IPreparedFeature Prefab()
            => Feature.Create(() => new GameObjectManagerActor(),
                              () => new GomState(ImmutableDictionary<Type, CommandProcessorBase>.Empty, ImmutableDictionary<string, GameObject>.Empty));

        private const string GlobalGameObject = "Game_Global_GameObject_Container";


        protected override void Config()
        {
            Receive<IGameCommand>(RunCommand);
            Receive<RegisterCommandProcessorBase>(obs => obs.Select(s => s.State with{ CommandProcessors = s.State.CommandProcessors.Add(s.Event.CommandType, s.Event.CreateProcessor())}));

            Receive<RequestGameComponent>(obs => obs.Select(s =>
                                                            {
                                                                var (requestGameComponent, state, _) = s;
                                                                if (state.GameObjects.TryGetValue(GlobalGameObject, out var gameObject) && gameObject.HasComponent(requestGameComponent.Type))
                                                                    return new RespondGameComponent(gameObject.GetComponent(requestGameComponent.Type));
                                                                return new RespondGameComponent(null);
                                                            }).ToSender());

            Receive<ComponentBlueprint>(obs => obs.Select(statePair => statePair.State.GameObjects.TryGetValue(GlobalGameObject, out var target) 
                                                                           ? (statePair, target) 
                                                                           : (statePair, new GameObject(GlobalGameObject, ImmutableDictionary<Type, ComponentObject>.Empty)))
                                                  .Select(d =>
                                                          {
                                                              var ((evt, state, _), obj) = d;

                                                              var comp = CreateComponent(evt);
                                                              obj = obj with {Components = obj.Components.Add(comp.ComponentType, comp)};

                                                              return state with {GameObjects = state.GameObjects.SetItem(GlobalGameObject, obj)};
                                                          }));

            Receive<UpdateData>(obs => obs.Finally());

            base.Config();
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

        private IDisposable RunCommand(IObservable<StatePair<IGameCommand, GomState>> commandObservable)
        {
            IEnumerable<(GameObject obj, object componet)> GetComponents(Type targetType, IGameCommand command, ImmutableDictionary<string, GameObject> gameObjects)
            {
                if (string.IsNullOrWhiteSpace(command.Target))
                {
                    foreach (var gameObject in gameObjects.Select(p => p.Value).Where(gameObject => gameObject.HasComponent(targetType)))
                        yield return (gameObject, gameObject.GetComponent(targetType));
                }
                else if (gameObjects.TryGetValue(command.Target, out var gameObject) && gameObject.HasComponent(targetType))
                    yield return (gameObject, gameObject.GetComponent(targetType));
            }

            return commandObservable.SelectMany(statePair => statePair.State.CommandProcessors.Lookup(statePair.Event.GetType()).Select(processorBase => (processorBase, statePair)))
                                    .SelectMany(processor => GetComponents(processor.processorBase.Component, processor.statePair.Event, processor.statePair.State.GameObjects)
                                                   .Select(component => (component, processor.processorBase, processor.statePair.Event)))
                                    .Where(processor => processor.processorBase.CanProcess(processor.component.componet))
                                    .ToUnit(processor => processor.processorBase.Run(Game, processor.component.componet, processor.component.obj, processor.Event))
                                    .SubscribeWithStatus();
        }

        public GameObjectManagerActor()
        {
            Receive<GameSetup>(SetupGame);

            Receive<FillData>(FillSaveData);
            Receive<UpdateData>(UpdateSaveData);
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