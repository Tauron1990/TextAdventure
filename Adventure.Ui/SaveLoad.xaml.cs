using System;
using System.IO;
using System.Windows.Input;
using JetBrains.Annotations;

namespace Adventure.Ui
{
    /// <summary>
    /// Interaktionslogik für SaveLoad.xaml
    /// </summary>
    public partial class SaveLoad
    {
        private sealed class InternalModel
        {
            private sealed class MiniCommand : ICommand
            {
                private readonly Action<object> _exec;

                public MiniCommand(Action<object> exec) => _exec = exec;

                public bool CanExecute(object parameter) => true;

                public void Execute(object parameter) => _exec(parameter);

                public event EventHandler CanExecuteChanged
                {
                    add { }
                    remove { }
                }
            }

            public InternalModel(Action<string> loadGame, Action<string> newGame)
            {
                LoadGame = new MiniCommand(o => loadGame((string)o));
                NewGame = new MiniCommand(o => newGame((string)o));
            }

            [UsedImplicitly]
            public ICommand LoadGame { get; }

            [UsedImplicitly]
            public ICommand NewGame { get; }
        }

        private string _loaction = string.Empty;

        public event Action<string>? LoadGameEvent; 

        public SaveLoad() => InitializeComponent();

        public void Init(string saveGameLocation)
        {
            _loaction = saveGameLocation;
            DataContext = new InternalModel(LoadGame, NewGame);
        }

        private void NewGame(string obj)
        {
            var fullPath = Path.GetFullPath(Path.Combine(_loaction, obj + ".sav"));
            if(File.Exists(fullPath))
                File.Delete(fullPath);

            LoadGameEvent?.Invoke(fullPath);
        }

        private void LoadGame(string obj)
        {
            var fullPath = Path.GetFullPath(Path.Combine(_loaction, obj + ".sav"));
            LoadGameEvent?.Invoke(fullPath);
        }
    }
}
