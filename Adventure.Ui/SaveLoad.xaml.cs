using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Adventure.GameEngine;

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

            public ICommand LoadGame { get; }

            public ICommand NewGame { get; }
        }

        public SaveLoad() => InitializeComponent();

        public void Init(string saveGameLocation)
        {
            DataContext = new InternalModel(LoadGame, NewGame);
        }

        private void NewGame(string obj)
        {
            
        }

        private void LoadGame(string obj)
        {
        }
    }
}
