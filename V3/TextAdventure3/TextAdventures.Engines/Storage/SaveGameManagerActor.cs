using System;
using System.Collections.Immutable;
using System.IO;
using Akka.Actor;
using Newtonsoft.Json;
using TextAdventures.Engine.Actors;

namespace TextAdventures.Engine.Storage
{
    public sealed class SaveGameManagerActor : GameProcess
    {
        private readonly GameProfile _profile;
        private readonly IActorRef _objectManager;

        public SaveGameManagerActor(GameProfile profile, IActorRef objectManager)
        {
            _profile = profile;
            _objectManager = objectManager;

            Receive<MakeSaveGame>(RunSave);
            Receive<FillData>(DataFilled);
        }

        private static void DataFilled(FillData obj)
        {
            var (targetPath, objectData) = obj;
            File.WriteAllText(targetPath, JsonConvert.SerializeObject(objectData));
        }

        private void RunSave(MakeSaveGame obj)
        {
            var profile = _profile.GetSaveGame(obj.Name);
            profile.Save();

            string saveGamePath = profile.Saves[obj.Name];
            _objectManager.Tell(new FillData(saveGamePath, new ObjectData(ImmutableDictionary<string, ComponentData>.Empty)));
        }
    }

    public sealed record ComponentData(Type ComponentType, ImmutableDictionary<string, string> Propertys);

    public sealed record ObjectData(ImmutableDictionary<string, ComponentData> Objects);

    internal sealed record FillData(string TargetPath, ObjectData Data);
}