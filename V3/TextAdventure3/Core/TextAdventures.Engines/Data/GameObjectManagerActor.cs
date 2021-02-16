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
        private const string GlobalGameObject = "Game_Global_GameObject_Container";
        
        public static IPreparedFeature Prefab()
            => Feature.Create(() => new GameObjectManagerActor(),
                () => new GomState(ImmutableDictionary<Type, CommandProcessorBase>.Empty,
                    ImmutableDictionary<string, GameObject>.Empty));


        protected override void Config()
        {
            Receive<IGameCommand>(RunCommand);
            Receive<RegisterCommandProcessorBase>(obs => obs.Select(s => s.State with
                                                                         {
                                                                             CommandProcessors =
                                                                             s.State.CommandProcessors.Add(
                                                                                 s.Event.CommandType,
                                                                                 s.Event.CreateProcessor())
                                                                         }));

            Receive<RequestGameComponent>(obs => obs.Select(s =>
                                                            {
                                                                var (requestGameComponent, state, _) = s;
                                                                if (state.GameObjects.TryGetValue(GlobalGameObject,
                                                                        out var gameObject) &&
                                                                    gameObject.HasComponent(requestGameComponent.Type))
                                                                    return new RespondGameComponent(
                                                                        gameObject.GetComponent(requestGameComponent
                                                                           .Type));
                                                                return new RespondGameComponent(null);
                                                            }).ToSender());

            Receive<ComponentBlueprint>(
                obs => obs.Select(statePair => statePair.State.GameObjects.TryGetValue(GlobalGameObject, out var target)
                                      ? (statePair, target)
                                      : (statePair,
                                         new GameObject(GlobalGameObject,
                                             ImmutableDictionary<Type, ComponentObject>.Empty)))
                          .Select(d =>
                                  {
                                      var ((evt, state, _), obj) = d;

                                      var comp = CreateComponent(evt);
                                      obj = obj with {Components = obj.Components.Add(comp.ComponentType, comp)};

                                      return state with
                                             {
                                                 GameObjects = state.GameObjects.SetItem(GlobalGameObject, obj)
                                             };
                                  }));

            Receive<UpdateData>(
                obs => obs
                      .Select(evt =>
                                  new
                                  {
                                      EventPair = evt,
                                      Data = ImmutableDictionary<string, GameObject>
                                            .Empty
                                            .AddRange(Observable.Return(evt)
                                                                .SelectMany(data => data.Event.Data.Select(obj => (obj, data)))
                                                                .Select(s =>
                                                                        {
                                                                            var (objData, info) = s;
                                                                            if (!info.State.GameObjects.TryGetValue(objData.Key, out var gameObject))
                                                                                gameObject = new GameObject(objData.Key, ImmutableDictionary<Type, ComponentObject>.Empty);

                                                                            return (objData, (objData.Key, gameObject));
                                                                        })
                                                                .Select(e =>
                                                                        {
                                                                            var (objectData, (objectKey, gameObject)) = e;

                                                                            var compData =
                                                                                ImmutableDictionary<Type, ComponentObject>
                                                                                   .Empty.AddRange(objectData.Value
                                                                                                             .Select(cType =>
                                                                                                                         (cType.Key,
                                                                                                                          cType.Value,
                                                                                                                          gameObject.Components.GetValueOrDefault(cType.Key) ??
                                                                                                                          new ComponentObject(FastReflection.Shared
                                                                                                                                                            .FastCreateInstance(cType.Key))))
                                                                                                             .Select(info =>
                                                                                                                     {
                                                                                                                         var (key, value, componentObject) = info;

                                                                                                                         if (componentObject!.Component is ComponentBase componentBase)
                                                                                                                             componentBase.Cache = value;

                                                                                                                         return new KeyValuePair<Type, ComponentObject>(
                                                                                                                             key,
                                                                                                                             componentObject);
                                                                                                                     }));

                                                                            var newGameObject = gameObject with {Components = compData};

                                                                            return new KeyValuePair<string, GameObject>(objectKey, newGameObject);
                                                                        })
                                                                .ToEnumerable())
                                  })
                      .Do(i => Sender.Tell(i.EventPair.Event))
                      .Select(evt => evt.EventPair.State with {GameObjects = evt.Data}));


            Receive<FillData>(obs => obs
                                    .Select(p => (p.Event, Data:p.State.GameObjects
                                                                 .ToImmutableDictionary(
                                                                      c => c.Key,
                                                                      c => c.Value.Components
                                                                            .ToImmutableDictionary(
                                                                                 cc => cc.Key,
                                                                                 cc => cc.Value.Component is ComponentBase componentBase 
                                                                                     ? componentBase.Cache : ImmutableDictionary<string, object?>.Empty))))
                                    .Select(p => p.Event with{ Data = p.Data })
                                        .ToSender());

            base.Config();
        }

        private IDisposable RunCommand(IObservable<StatePair<IGameCommand, GomState>> commandObservable)
        {
            IEnumerable<(GameObject obj, object componet)> GetComponents(Type targetType, IGameCommand command,
                ImmutableDictionary<string, GameObject> gameObjects)
            {
                if (string.IsNullOrWhiteSpace(command.Target))
                    foreach (var gameObject in gameObjects.Select(p => p.Value)
                                                          .Where(gameObject => gameObject.HasComponent(targetType)))
                        yield return (gameObject, gameObject.GetComponent(targetType));
                else if (gameObjects.TryGetValue(command.Target, out var gameObject) &&
                         gameObject.HasComponent(targetType))
                    yield return (gameObject, gameObject.GetComponent(targetType));
            }

            return commandObservable.SelectMany(statePair =>
                                                    statePair.State.CommandProcessors
                                                             .Lookup(statePair.Event.GetType())
                                                             .Select(processorBase
                                                                         => (processorBase, statePair)))
                                    .SelectMany(processor =>
                                                    GetComponents(processor.processorBase.Component, processor.statePair.Event, processor.statePair.State.GameObjects)
                                                       .Select(component => (component, processor.processorBase, processor.statePair.Event)))
                                    .Where(processor => processor.processorBase.CanProcess(processor.component.componet))
                                    .ToUnit(processor => processor.processorBase.Run(Game, processor.component.componet, processor.component.obj, processor.Event))
                                    .SubscribeWithStatus();
        }


        //private void SetupGame(GameSetup setup)
        //{
        //    try
        //    {
        //        foreach (var blueprint in setup.GameObjectBlueprints)
        //        {
        //            var dic = blueprint.ComponentBlueprints
        //                               .Select(p => (p.ComponentType, CreateComponent(p)))
        //                               .ToImmutableDictionary(p => p.ComponentType, p => p.Item2);

        //            _gameObjects.Add(blueprint.Name, new GameObject(blueprint.Name, dic));
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        setup.Error(e);
        //    }
        //    finally
        //    {
        //        Sender.Tell(setup);
        //    }
        //}

        private static ComponentObject CreateComponent(ComponentBlueprint blueprint)
        {
            var inst = FastReflection.Shared.FastCreateInstance(blueprint.ComponentType);
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

        public record GomState(ImmutableDictionary<Type, CommandProcessorBase> CommandProcessors,
            ImmutableDictionary<string, GameObject> GameObjects);
    }
}