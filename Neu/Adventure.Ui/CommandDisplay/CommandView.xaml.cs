using System;
using System.Collections.Generic;
using Adventure.GameEngine;
using Adventure.GameEngine.Commands;
using Adventure.GameEngine.Core;

namespace Adventure.Ui.CommandDisplay
{
    /// <summary>
    /// Interaktionslogik für CommandView.xaml
    /// </summary>
    public partial class CommandView
    {
        private readonly CommandViewModel _model;

        public CommandView()
        {
            InitializeComponent();

            _model = new CommandViewModel(c => CommandSelect?.Invoke(c));
            DataContext = _model;
        }

        public event Action<Command>? CommandSelect; 

        public void InitGame(Game game)
            => _model.NewContent(game.Content);

        public void UnloadGame()
            => _model.Clear();

        public void Update(IEnumerable<(LazyString, Command)> commands)
            => _model.UpdateCommands(commands);
    }
}
