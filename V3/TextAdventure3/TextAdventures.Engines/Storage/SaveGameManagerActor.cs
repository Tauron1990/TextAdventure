using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using Akka.Actor;
using Newtonsoft.Json;
using TextAdventures.Builder;
using TextAdventures.Engine.Actors;

namespace TextAdventures.Engine.Storage
{
    public sealed class SaveGameManagerActor : GameProcess
    {
        private readonly IActorRef _objectManager;
        private readonly GameProfile _profile;

        public SaveGameManagerActor(GameProfile profile, IActorRef objectManager)
        {
            _profile = profile;
            _objectManager = objectManager;

            Receive<MakeSaveGame>(RunSave);
            Receive<FillData>(DataFilled);
            Receive<GameSetup>(SetupGame);
        }

        private void SetupGame(GameSetup obj)
        {
            ImmutableDictionary<string, object?> DeserializeComponent(ComponentData data)
                => data.Propertys.ToImmutableDictionary(d => d.Key,
                                                        d => JsonConvert.DeserializeObject(d.Value.Data, d.Value.Type));

            ImmutableDictionary<Type, ImmutableDictionary<string, object?>> DeserializeObject(ObjectData data) 
                => data.Components.ToImmutableDictionary(d => d.ComponentType, DeserializeComponent);

            if (_profile.Saves.TryGetValue(obj.SaveGame, out var path))
            {
                var list = JsonConvert.DeserializeObject<ObjectList>(File.ReadAllText(path));

                var sender = Sender;

                _objectManager.Ask<UpdateData>(new UpdateData(list.Objects.ToImmutableDictionary(o => o.Key, o => DeserializeObject(o.Value))))
                              .ContinueWith(_ => sender.Tell(obj));
            }
            else
                Sender.Tell(obj);
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

            ObjectList SerializeList(ImmutableDictionary<string, ImmutableDictionary<Type, ImmutableDictionary<string, object?>>> list)
                => new(list.Select(p => (p.Key, SerializeObject(p.Value)))
                           .ToImmutableDictionary(p => p.Key, p => p.Item2));

            var (targetPath, objectDatas) = obj;

            File.WriteAllText(targetPath, JsonConvert.SerializeObject(SerializeList(objectDatas)));
        }

        private void RunSave(MakeSaveGame obj)
        {
            var profile = _profile.GetSaveGame(obj.Name);
            profile.Save();

            string saveGamePath = profile.Saves[obj.Name];
            _objectManager.Tell(new FillData(saveGamePath, ImmutableDictionary<string, ImmutableDictionary<Type, ImmutableDictionary<string, object?>>>.Empty));
        }
    }

    public sealed record ObjectList(ImmutableDictionary<string, ObjectData> Objects);

    public sealed record ComponentData(Type ComponentType, ImmutableDictionary<string, ObjectProperty> Propertys);

    public sealed record ObjectData(ImmutableList<ComponentData> Components);

    public sealed record ObjectProperty(Type Type, string Data);

    internal sealed record FillData(string TargetPath, ImmutableDictionary<string, ImmutableDictionary<Type, ImmutableDictionary<string, object?>>> Data);

    internal sealed record UpdateData(ImmutableDictionary<string, ImmutableDictionary<Type, ImmutableDictionary<string, object?>>> Data);
}