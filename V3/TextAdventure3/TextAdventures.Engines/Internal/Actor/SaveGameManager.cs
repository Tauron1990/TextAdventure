using System;
using Akka.Actor;
using Microsoft.Data.Sqlite;
using Tauron.Akka;
using TextAdventures.Engine.Commands;
using TextAdventures.Engine.Internal.Data;
using TextAdventures.Engine.Internal.Messages;
using TextAdventures.Engine.Querys.Result;

namespace TextAdventures.Engine.Internal.Actor
{
    public sealed class SaveGameManager : ExposedReceiveActor, IWithTimers
    {
        private SaveProfile? _saveProfile;
        private SaveInfo? _current;

        public ITimerScheduler Timers { get; set; } = null!;

        public SaveGameManager()
        {
            Receive<StartGame>(Initialize);
            Receive<StartAutoSave>(MakeAutoSave);
            Receive<SaveGameCommand>(MakeSaveFile);
        }

        private void MakeAutoSave(StartAutoSave obj)
        {
            if (_saveProfile == null)
                return;

            var saveName = "autorsave_" + _saveProfile.AutoSaveIncrement;
            _saveProfile.AutoSaveIncrement++;
            if (_saveProfile.AutoSaveIncrement < 5)
                _saveProfile.AutoSaveIncrement = 1;

            Self.Tell(new SaveGameCommand(saveName));
        }

        private void MakeSaveFile(SaveGameCommand obj)
        {
            try
            {
                if(_saveProfile == null)
                    return;

                var info = GetSaveFile(obj.Name);

                using var main = new SqliteConnection(_saveProfile.GetConnectionString());
                using var saveFile = new SqliteConnection(_saveProfile.GetConnectionString(info));

                main.Open();
                saveFile.Open();

                main.BackupDatabase(saveFile);

                info.SaveTime = DateTime.Now;
                info.Profile.Save();

                if(Sender != null && !Sender.IsNobody() && !Sender.Equals(Self))
                    Sender.Tell(QueryResult.Compleded(true));
            }
            catch (Exception e)
            {
                Log.Error(e, "Error on Save File");
                if (Sender != null && !Sender.IsNobody() && !Sender.Equals(Self))
                    Sender.Tell(QueryResult.Error(e));
            }
        }

        private void Initialize(StartGame start)
        {
            try
            {
                _saveProfile = start.SaveGame;
                Timers.StartPeriodicTimer(StartAutoSave.Inst, StartAutoSave.Inst, _saveProfile.AutoSaveInterval);

                if (start.SaveInfo == null)
                {
                    Sender.Tell(QueryResult.Compleded(true));
                    return;
                }

                _current = start.SaveInfo;

                var conMain = start.SaveGame.GetConnectionString();
                var save = start.SaveGame.GetConnectionString(start.SaveInfo);

                using var main = new SqliteConnection(conMain);
                using var from = new SqliteConnection(save);

                main.Open();
                from.Open();

                from.BackupDatabase(main);

                Sender.Tell(QueryResult.Compleded(true));
            }
            catch (Exception e)
            {
                Log.Error(e, "Error on Load Game");
                Sender.Tell(QueryResult.Error(e));
            }
        }

        private SaveInfo GetSaveFile(string? name)
        {
            SaveInfo GetOrCreate(string internalName) 
                => _saveProfile!.GetSave(internalName) ?? _saveProfile!.NewSaveFile(internalName);

            if (string.IsNullOrWhiteSpace(name))
                return _current ?? GetOrCreate("default");
            return GetOrCreate(name);
        }

        private sealed class StartAutoSave
        {
            public static readonly StartAutoSave Inst = new StartAutoSave();
        }
    }
}