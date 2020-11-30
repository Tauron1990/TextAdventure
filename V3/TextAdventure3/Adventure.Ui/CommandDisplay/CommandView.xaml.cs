using System;
using System.Collections.Generic;

namespace Adventure.Ui.CommandDisplay
{
    /// <summary>
    ///     Interaktionslogik für CommandView.xaml
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

        public event Action<IGameCommand>? CommandSelect;

        public void UnloadGame()
            => Dispatcher.Invoke(_model.Clear);

        public void Update(IEnumerable<(ICommandMetadata?, IGameCommand)> commands)
            => Dispatcher.Invoke(() => _model.UpdateCommands(commands));
    }
}