using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Adventure.Ui.Internal;
using Adventure.Ui.WpfCommands;
using JetBrains.Annotations;
using Tauron;
using Tauron.Application;
using Tauron.Features;
using TextAdventures.Builder;
using TextAdventures.Engine;
using TextAdventures.Engine.Actors;
using TextAdventures.Engine.Storage;

namespace Adventure.Ui
{
    /// <summary>
    ///     Interaktionslogik für SaveLoad.xaml
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
            _model.GameName = saveGameLocations;
            saveGameLocations.CreateDirectoryIfNotExis();
        }

        [PublicAPI]
        public event Action<string, string?, bool>? NewGame;

        [PublicAPI]
        public void Prepare(World world) => world.WithProcess("Save_Load_Subscriber", SaveLoadSubscriber.Create(GameLoaded));

        private void GameLoaded(LoadingCompled gl)
        {
            var ((_, _, gameMaster), gameProfile) = gl;

            gameMaster
               .WhenTerminated
               .ContinueWith(_ =>
                             {
                                 _model.IsGameRunning = false;
                                 _model.GameMaster = null;
                                 _model.Profile = null;
                             });

            _model.GameMaster = gameMaster;
            _model.IsGameRunning = true;
            _model.Profile = gameProfile;
        }

        [UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature)]
        private sealed class SaveLoadSubscriber : GameProcess<SaveLoadSubscriber.SlsState>
        {
            public sealed record SlsState(Action<LoadingCompled> GameLoaded);

            public static IPreparedFeature Create(Action<LoadingCompled> gameLoaded)
                => Feature.Create(() => new SaveLoadSubscriber(), () => new SlsState(gameLoaded));


            protected override void LoadingCompled(StatePair<LoadingCompled, SlsState> message)
            {
                base.LoadingCompled(message);

                CurrentState.GameLoaded(message.Event);
            }
        }
    }

    internal sealed class SaveLoadModel : ObservableObject
    {
        private static readonly char[] InvalidChars = Path.GetInvalidFileNameChars();
        private readonly Dispatcher _dispatcher;

        private readonly Action<string, string?, bool> _starter;
        private List<string>? _blockedNames;
        private GameMaster? _gameMaster;
        private bool _isGameRunning;
        private NameInfo _isNewNameOk;
        private NameInfo _isSaveGameNameOk;
        private string _newNameText = string.Empty;
        private GameProfile? _profile;
        private string _gameName;

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

        public GameProfile? Profile
        {
            get => _profile;
            set
            {
                if (Equals(value, _profile)) return;
                _profile = value;
                OnPropertyChanged();
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

        public ObservableCollection<GameProfile> Profiles { get; } = new();

        public string GameName
        {
            get => _gameName;
            set
            {
                _gameName = value;
                UpdateSaveGames();
            }
        }

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

        public ICommand GenericLoadGame { get; }

        public NameInfo IsSaveGameNameOk
        {
            get => _isSaveGameNameOk;
            set
            {
                if (value == _isSaveGameNameOk) return;
                _isSaveGameNameOk = value;
                OnPropertyChanged();
            }
        }

        public ICommand GenericSvaeGame { get; }

        public SaveLoadModel(Action<string, string?, bool> starter, Dispatcher dispatcher, string gameName)
        {
            _gameName = gameName;
            _starter = starter;
            _dispatcher = dispatcher;
            NewName = new SimpleCommand(() => IsNewNameOk != NameInfo.Error, NewGame);
            GenericLoadGame = new SimpleCommand(CanLoad, ExcuteLoad);
            GenericSvaeGame = new SimpleCommand(_ => IsGameRunning && IsSaveGameNameOk != NameInfo.Error, ExcuteSave);
        }

        private NameInfo ValidateNewName()
        {
            if (string.IsNullOrWhiteSpace(NewNameText) || NewNameText.Contains('.') || NewNameText.Any(InvalidChars.Contains))
                return NameInfo.Error;
            
            _blockedNames ??= new List<string>(GameProfile.GetProfiles(GameName).Select(sp => sp.Name));

            return _blockedNames.Contains(NewNameText) ? NameInfo.Warning : NameInfo.Ok;
        }

        private void NewGame()
        {
            var name = NewNameText;
            NewNameText = string.Empty;

            _blockedNames = null;
            if (IsGameRunning && GameMaster != null)
            {
                GameMaster
                   .Stop()
                   .ContinueWith(_ => _starter(name, null, true));
            }
            else
                _starter(name, null, true);
        }

        private void UpdateSaveGames()
        {
            Task.Run(() =>
                     {
                         _dispatcher.Invoke(Profiles.Clear);
                         var profiles = SaveProfile.GetProfiles(GameName);

                         _dispatcher.BeginInvoke(new Action(() =>
                                                            {
                                                                foreach (var profile in profiles)
                                                                    Profiles.Add(profile);
                                                            }));
                     });
        }



        private bool CanLoad(object? arg)
        {

        }

        private void ExcuteLoad(object? target)
        {
            void ExecuteAction(Action action)
            {
                if (IsGameRunning && GameMaster != null)
                {
                    GameMaster.Stop()
                              .ContinueWith(e => action());
                }
                else
                    action();
            }

            if (target == null) return;

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

        private NameInfo ValidateSaveGameInfo()
        {
            if (SaveGameName.Any(InvalidChars.Contains))
                return NameInfo.Error;

            return Profile?.Saves.Any(z => z.Name == SaveGameName) == true ? NameInfo.Warning : NameInfo.Ok;
        }

        private async void ExcuteSave(object? target)
        {
            if (GameMaster == null)
                return;

            string targetName = target switch
            {
                null => SaveGameName,
                SaveInfo info => info.Name,
                _ => string.Empty
            };

            var result = await GameMaster.SendSave(new SaveGameCommand(targetName));
            if (result is QueryFailed failed)
                MessageBox.Show(Application.Current.MainWindow!, failed.Error.ToString());

            OnPropertyChangedExplicit(nameof(Profile));
        }
    }
}