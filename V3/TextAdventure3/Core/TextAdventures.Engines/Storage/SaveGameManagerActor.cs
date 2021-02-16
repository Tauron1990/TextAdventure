using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using Akka.Actor;
using Newtonsoft.Json;
using Tauron;
using Tauron.Features;
using TextAdventures.Builder;
using TextAdventures.Engine.Actors;

namespace TextAdventures.Engine.Storage
{
    public sealed class SaveGameManagerActor : GameProcess<SaveGameManagerActor.SgmState>
    {
        public static IPreparedFeature Prefab(GameProfile profile, IActorRef objectManager)
            => Feature.Create(() => new SaveGameManagerActor(), () => new SgmState(objectManager, profile));

        protected override void Config()
        {
            base.Config();

            Receive<GameSetup>(obs => obs.SelectMany(SetupGame).ToSender());
            Receive<FillData>(obs => obs.Select(s => s.Event).SubscribeWithStatus(DataFilled));
            Receive<MakeSaveGame>(obs => obs.Select(RunSave));
        }

        private static IObservable<GameSetup> SetupGame(StatePair<GameSetup, SgmState> incomming)
        {
            var (gameSetup, (objectManager, profile), _) = incomming;

            ImmutableDictionary<string, object?> DeserializeComponent(ComponentData data)
                => data.Propertys.ToImmutableDictionary(d => d.Key,
                    d => JsonConvert.DeserializeObject(d.Value.Data, d.Value.Type));

            ImmutableDictionary<Type, ImmutableDictionary<string, object?>> DeserializeObject(ObjectData data)
                => data.Components.ToImmutableDictionary(d => d.ComponentType, DeserializeComponent);

            if (!profile.Saves.TryGetValue(gameSetup.SaveGame, out var path)) return Observable.Return(gameSetup);

            var list = JsonConvert.DeserializeObject<ObjectList>(File.ReadAllText(path));

            return objectManager
                  .Ask<UpdateData>(
                       new UpdateData(list.Objects.ToImmutableDictionary(o => o.Key, o => DeserializeObject(o.Value))))
                  .ToObservable().Select(_ => gameSetup);
        }

        private static void DataFilled(FillData obj)
        {
            ObjectProperty SerilaizeProperty(object value) => new(value.GetType(), JsonConvert.SerializeObject(value));

            ComponentData SerializeComponent(Type type, ImmutableDictionary<string, object?> data)
                => new(type, data
                            .Where(o => o.Value != null)
                            .Select(o => (o.Key, SerilaizeProperty(o.Value!)))
                            .ToImmutableDictionary(p => p.Key, p => p.Item2));

            ObjectData SerializeObject(ImmutableDictionary<Type, ImmutableDictionary<string, object?>> components)
                => new(components.Select(p => SerializeComponent(p.Key, p.Value))
                                 .ToImmutableList());

            ObjectList SerializeList(
                ImmutableDictionary<string, ImmutableDictionary<Type, ImmutableDictionary<string, object?>>> list)
                => new(list.Select(p => (p.Key, SerializeObject(p.Value)))
                           .ToImmutableDictionary(p => p.Key, p => p.Item2));

            var (targetPath, objectDatas) = obj;

            File.WriteAllText(targetPath, JsonConvert.SerializeObject(SerializeList(objectDatas)));
        }

        private static SgmState RunSave(StatePair<MakeSaveGame, SgmState> incomming)
        {
            var (obj, (objectManager, gameProfile), _) = incomming;

            var profile = gameProfile.GetSaveGame(obj.Name);
            profile.Save();

            string saveGamePath = profile.Saves[obj.Name];
            objectManager.Tell(new FillData(saveGamePath,
                ImmutableDictionary<string, ImmutableDictionary<Type, ImmutableDictionary<string, object?>>>.Empty));

            return incomming.State with {Profile = profile};
        }

        public sealed record SgmState(IActorRef ObjectManager, GameProfile Profile);
    }

    public sealed record ObjectList(ImmutableDictionary<string, ObjectData> Objects);

    public sealed record ComponentData(Type ComponentType, ImmutableDictionary<string, ObjectProperty> Propertys);

    public sealed record ObjectData(ImmutableList<ComponentData> Components);

    public sealed record ObjectProperty(Type Type, string Data);

    internal sealed record FillData(string TargetPath, ImmutableDictionary<string, ImmutableDictionary<Type, ImmutableDictionary<string, object?>>> Data);

    internal sealed record UpdateData(ImmutableDictionary<string, ImmutableDictionary<Type, ImmutableDictionary<string, object?>>> Data);
}