using System;
using System.Windows.Input;
using JetBrains.Annotations;

namespace Tauron.Application.CommonUI.Commands
{
    [PublicAPI]
    public abstract class CommandBase : ICommand
    {
        public event EventHandler? CanExecuteChanged
        {
            add { throw new NotImplementedException(); }
            remove { throw new NotImplementedException(); }
        }

        public virtual bool CanExecute(object? parameter) => true;

        public abstract void Execute(object? parameter);

        public virtual void RaiseCanExecuteChanged() 
            => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}