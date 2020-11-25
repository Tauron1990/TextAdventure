using System;
using System.IO;
using Akka.Actor;
using Akka.Configuration;
using JetBrains.Annotations;
using TextAdventures.Builder;
using TextAdventures.Builder.Internal;
using TextAdventures.Engine.Internal.Actor;
using TextAdventures.Engine.Internal.Data;
using TextAdventures.Engine.Internal.Messages;

namespace TextAdventures.Engine
{
    [PublicAPI]
    public sealed class Game
    {
        private readonly bool _newGame;

        private readonly WorldImpl _world;

        private Game(World world, bool newGame)
        {
            _world   = (WorldImpl) world;
            _newGame = newGame;
        }

        public static Game Create(World world, bool newGame)
            => new(world, newGame);

        public GameMaster Start(string? saveGameName, Action<Exception> fail)
        {
            var info = SaveProfile.Get(_world.DataPath, _world.ProfileName);
            Environment.CurrentDirectory = _world.DataPath;

            var connectionString = info.GetConnectionString();

            var connectionConfig =
                $"akka.persistence.journal.sqlite.connection-string : \"{connectionString}\"\n" +
                $"akka.persistence.snapshot-store.sqlite.connection-string : \"{connectionString}\"";

            var system = ActorSystem.Create("TextAdventures",
                                            ConfigurationFactory.ParseString(connectionConfig).WithFallback(ConfigurationFactory.FromResource<Game>("TextAdventures.Engine.akka.conf")));
            var gameMaster = system.ActorOf(Props.Create(() => new GameMasterActor(fail)), "GameMaster");

            if (_newGame)
                info.ClearSaves();

            gameMaster.Tell(new StartGame(_world, _newGame || !File.Exists(Path.Combine(_world.DataPath, SaveProfile.ProfileFile)), info, info.GetSave(saveGameName)));
            return new GameMaster(gameMaster, system);
        }
    }
}