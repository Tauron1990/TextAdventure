using System;
using System.IO;
using Akka.Actor;
using Akka.Configuration;
using JetBrains.Annotations;
using Microsoft.Data.Sqlite;
using TextAdventures.Builder;
using TextAdventures.Builder.Internal;
using TextAdventures.Engine.Internal.Actor;
using TextAdventures.Engine.Internal.Messages;

namespace TextAdventures.Engine
{
    [PublicAPI]
    public sealed class Game
    {
        public static Game Create(World world, bool newGame)
            => new Game(world, newGame);

        private readonly WorldImpl _world;
        private readonly bool _newGame;

        private Game(World world, bool newGame)
        {
            _world = (WorldImpl)world;
            _newGame = newGame;
        }

        public GameMaster Start()
        {
            var targetDic = Path.GetDirectoryName(_world.SaveGame);
            var targetFile = Path.GetFileName(_world.SaveGame);
            if(string.IsNullOrWhiteSpace(targetDic) || string.IsNullOrWhiteSpace(targetFile))
                throw new InvalidOperationException("The Save game file is not an Valid Full Path");

            Environment.CurrentDirectory = targetDic;
            var connectionStringBuilder = new SqliteConnectionStringBuilder { DataSource = targetFile, Mode = SqliteOpenMode.ReadWriteCreate, Cache = SqliteCacheMode.Shared };
            var connectionConfig = 
                $"akka.persistence.journal.sqlite.connection-string : \"{connectionStringBuilder.ConnectionString}\"\n" +
                $"akka.persistence.snapshot-store.sqlite.connection-string : \"{connectionStringBuilder.ConnectionString}\"";

            var system = ActorSystem.Create("TextAdventures",
                ConfigurationFactory.ParseString(connectionConfig).WithFallback(ConfigurationFactory.FromResource<Game>("TextAdventures.Engine.akka.conf")));
            var gameMaster = system.ActorOf<GameMasterActor>("GameMaster");

            if(_newGame && File.Exists(_world.SaveGame))
                File.Delete(_world.SaveGame);

            gameMaster.Tell(new StartGame(_world, _newGame || !File.Exists(_world.SaveGame)));
            return new GameMaster(gameMaster, system);
        }
    }
}