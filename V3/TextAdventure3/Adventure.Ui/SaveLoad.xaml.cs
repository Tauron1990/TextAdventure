using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using Adventure.Ui.Internal;
using Adventure.Ui.WpfCommands;
using Akka.Actor;
using Akkatecture.Aggregates;
using Akkatecture.Subscribers;
using JetBrains.Annotations;
using Tauron;
using Tauron.Application;
using TextAdventures.Builder;
using TextAdventures.Engine;
using TextAdventures.Engine.Events;
using TextAdventures.Engine.Internal.Data;
using TextAdventures.Engine.Internal.Data.Aggregates;

namespace Adventure.Ui
{
    /// <summary>
    /// Interaktionslogik für SaveLoad.xaml
    /// </summary>
    public partial class SaveLoad
    {
        private readonly SaveLoadModel _model;
        
        public SaveLoad()
        {
            InitializeComponent();
            _model = new SaveLoadModel((s1, s2, b) => NewGame?.Invoke(s1, s2, b), Dispatcher.CurrentDispatcher);
            DataContext = _model;
        }

        [PublicAPI]
        public void Init(string saveGameLocations)
        {
            _model.SaveGmeLocation = saveGameLocations;
        }

        [PublicAPI]
        public event Action<string, string?, bool>? NewGame; 

        [PublicAPI]
        public void Prepare(World world)
        {
            world.Add(Props.Create<SaveLoadSubscriber>(new Action<GameLoaded>(GameLoaded)), "Save_Load_Subscriber");
        }

        private void GameLoaded(GameLoaded gl)
        {
            gl.Master
                .WhenTerminated
                .ContinueWith(t => _model.IsGameRunning = false);

            _model.GameMaster = gl.Master;
            _model.IsGameRunning = true;
        }

        [UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature)]
        private sealed class SaveLoadSubscriber : DomainEventSubscriber,
            ISubscribeTo<GameInfo, GameInfoId, GameLoaded>
        {
            private readonly Action<GameLoaded> _gameLoaded;

            public SaveLoadSubscriber(Action<GameLoaded> gameLoaded)
            {
                _gameLoaded = gameLoaded;
            }

            public bool Handle(IDomainEvent<GameInfo, GameInfoId, GameLoaded> domainEvent)
            {
                _gameLoaded(domainEvent.AggregateEvent);
                return true;
            }
        }
    }

    internal sealed class SaveLoadModel : ObservableObject
    {
        private static readonly char[] InvalidChars = Path.GetInvalidFileNameChars();

        private readonly Action<string, string?, bool> _starter;
        private readonly Dispatcher _dispatcher;
        private NameInfo _isNewNameOk;
        private bool _isGameRunning;
        private List<string>? _blockedNames;
        private string _newNameText = string.Empty;
        private GameMaster? _gameMaster;

        public GameMaster? GameMaster
        {
            get => _gameMaster;
            set
            {
                if (Equals(value, _gameMaster)) return;
                _gameMaster = value;
                OnPropertyChanged();
                UpdateSaveGames();
            }
        }

        public NameInfo IsNewNameOk
        {
            get => _isNewNameOk;
            set
            {
                if (value == _isNewNameOk) return;
                _isNewNameOk = value;
                OnPropertyChanged();
            }
        }

        public ICommand NewName { get; }

        public ObservableCollection<SaveProfile> Profiles { get; } = new ObservableCollection<SaveProfile>();

        public string SaveGmeLocation { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        public bool IsGameRunning
        {
            get => _isGameRunning;
            set
            {
                if (value == _isGameRunning) return;
                _isGameRunning = value;
                OnPropertyChanged();
            }
        }

        public string NewNameText
        {
            get => _newNameText;
            set
            {
                if (value == _newNameText) return;
                _newNameText = value;
                OnPropertyChanged();
                IsNewNameOk = ValidateNewName();
            }
        }

        public SaveLoadModel(Action<string, string?, bool> starter, Dispatcher dispatcher)
        {
            _starter = starter;
            _dispatcher = dispatcher;
            NewName = new SimpleCommand(() => IsNewNameOk != NameInfo.Error, NewGame);
            GenericLoadGame = new SimpleCommand(ExcuteLoad);
            UpdateSaveGames();
        }

        private NameInfo ValidateNewName()
        {
            if (string.IsNullOrWhiteSpace(NewNameText) || NewNameText.Contains('.') || NewNameText.Any(InvalidChars.Contains))
                return NameInfo.Error;

            _blockedNames ??= new List<string>(SaveProfile.GetProfiles(SaveGmeLocation).Select(sp => sp.Name));

            return _blockedNames.Contains(NewNameText) ? NameInfo.Warning : NameInfo.Ok;
        }

        private void NewGame()
        {
            _blockedNames = null;
            if (IsGameRunning && GameMaster != null)
            {
                GameMaster
                    .Stop()
                    .ContinueWith(_ => _starter(NewNameText, null, true));
            }
            else
                _starter(NewNameText, null, true);
        }

        private void UpdateSaveGames()
        {
            Task.Run(() =>
            {
                _dispatcher.Invoke(Profiles.Clear);
                var profiles = SaveProfile.GetProfiles(SaveGmeLocation);

                _dispatcher.Invoke(() => profiles.Foreach(p => Profiles.Add(p)));
            });
        }

        public ICommand GenericLoadGame { get; }

        private void ExcuteLoad(object? target)
        {
            void ExecuteAction(Action action)
            {
                if (IsGameRunning && GameMaster != null)
                    GameMaster.Stop()
                        .ContinueWith(e => action());
                else
                    action();
            }

            if(target == null) return;

            switch (target)
            {
                case SaveInfo info:
                    ExecuteAction(() => _starter(info.Profile.Name, info.Name, false));
                    break;
                case SaveProfile profile:
                    ExecuteAction(() => _starter(profile.Name, null, false));
                    break;
            }
        }

        public ICommand GenericSvaeGame { get; }

        private void ExcuteSave(object? target)
        {

        }
    }
}
