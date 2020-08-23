using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Adventure.GameEngine;
using Adventure.GameEngine.Commands;
using Adventure.GameEngine.ContentManagment;
using Adventure.GameEngine.Core;
using Adventure.Ui.WpfCommands;

namespace Adventure.Ui.CommandDisplay
{
    public sealed class CommandViewModel
    {
        private readonly ICommand _executor;

        private IContentManagement _contentManagement = new ContentManagement();

        public ObservableCollection<ICommandContent> Commands { get; } = new ObservableCollection<ICommandContent>();

        public CommandViewModel(Action<Command> exec)
        {
            _executor = new SimpleCommand(o =>
            {
                if (o != null)
                    exec((Command) o);
                else
                    MessageBox.Show("No Command", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            });
        }

        public CommandViewModel()
        {
            _executor = new SimpleCommand(() => { });
        }

        public void NewContent(IContentManagement contentManagement)
            => _contentManagement = contentManagement;

        public void Clear()
            => Commands.Clear();

        public void UpdateCommands(IEnumerable<(LazyString Name, Command Command)> commands)
        {
            void AddCommand(IEnumerable<(LazyString Name, Command Command)> commands, ICollection<ICommandContent> source)
            {
                foreach (var (name, command) in commands)
                    source.Add(new PotentalCommand(command, name.Format(_contentManagement), _executor));
            }

            Clear();

            foreach (var cats in commands
                .ToLookup(s => s.Name)
                .OrderBy(g => string.IsNullOrWhiteSpace(g.Key.Text)))
            {
                var cat = cats.Key.Format(_contentManagement);
                if (string.IsNullOrWhiteSpace(cat))
                    AddCommand(cats, Commands);

                var catImpl = new CommandCategory(cat);
                AddCommand(cats, catImpl.Commands);
                Commands.Add(catImpl);
            }
        }
    }
}