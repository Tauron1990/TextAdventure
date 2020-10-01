using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using Adventure.Ui.WpfCommands;
using TextAdventures.Builder.Data.Command;
using ICommand = System.Windows.Input.ICommand;

namespace Adventure.Ui.CommandDisplay
{
    public sealed class CommandViewModel
    {
        private readonly ICommand _executor;

        public ObservableCollection<ICommandContent> Commands { get; } = new ObservableCollection<ICommandContent>();

        public CommandViewModel(Action<IGameCommand> exec)
        {
            _executor = new SimpleCommand(o =>
            {
                if (o != null)
                    exec((IGameCommand) o);
                else
                    MessageBox.Show("No Command", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            });
        }

        public void Clear()
            => Commands.Clear();

        public void UpdateCommands(IEnumerable<(ICommandMetadata? Metadata, IGameCommand Command)> commands)
        {
            Clear();

            foreach (var (metadata, command) in commands)
                Commands.Add(new PotentalCommand(command, metadata?.Name ?? $"Unbekannt {command.GetType()}", _executor));
        }
    }
}