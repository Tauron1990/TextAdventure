﻿using System;
using System.Windows.Input;
using JetBrains.Annotations;

namespace Adventure.Ui.WpfCommands
{
    [PublicAPI]
    public class SimpleCommand : CommandBase
    {
        public static ICommand Empty = new SimpleCommand(() => { });

        private readonly Func<object?, bool>? _canExecute;

        private readonly Action<object?> _execute;
        private readonly object? _parameter;

        public SimpleCommand(Func<object?, bool>? canExecute, Action<object?> execute, object? parameter = null)
        {
            _canExecute = canExecute;
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _parameter = parameter;
        }

        public SimpleCommand(Action<object?> execute)
            : this(null, execute) { }

        public SimpleCommand(Func<bool>? canExecute, Action execute)
        {
            _execute = o => execute();
            if (canExecute != null)
                _canExecute = o => canExecute();
        }

        public SimpleCommand(Action execute)
            : this(null, execute) { }

        public override bool CanExecute(object? parameter)
        {
            parameter ??= _parameter;

            return _canExecute == null || _canExecute(parameter);
        }

        public override void Execute(object? parameter)
        {
            parameter ??= _parameter;
            _execute?.Invoke(parameter);
        }
    }
}