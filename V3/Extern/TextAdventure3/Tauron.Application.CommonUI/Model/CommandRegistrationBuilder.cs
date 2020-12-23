using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using JetBrains.Annotations;
using Tauron.Akka;

namespace Tauron.Application.CommonUI.Model
{
    [PublicAPI]
    public sealed class CommandRegistrationBuilder
    {
        private readonly Action<string, Action<object?>, IObservable<bool>?> _register;

        private List<IObservable<bool>> _canExecute = new();

        private Delegate? _command;

        internal CommandRegistrationBuilder(Action<string, Action<object?>, IObservable<bool>?> register, IExpandedReceiveActor target)
        {
            Target = target;
            _register = register;
        }

        public IExpandedReceiveActor Target { get; }

        public CommandRegistrationBuilder WithExecute(Action<object?> execute, IEnumerable<IObservable<bool>>? canExecute)
        {
            _command = Delegate.Combine(_command, execute);

            return canExecute != null ? WithCanExecute(canExecute) : this;
        }
        
        public CommandRegistrationBuilder WithExecute(Action execute, IEnumerable<IObservable<bool>> canExecute)
        {
            _command = Delegate.Combine(_command, new ActionMapper(execute).Action);

            return WithCanExecute(canExecute);
        }
        
        public CommandRegistrationBuilder WithExecute(Action<object?> execute, IObservable<bool>? canExecute)
        {
            _command = Delegate.Combine(_command, execute);

            return canExecute != null ? WithCanExecute(canExecute) : this;
        }

        public CommandRegistrationBuilder WithExecute(Action<object?> execute)
        {
            _command = Delegate.Combine(_command, execute);

            return this;
        }

        public CommandRegistrationBuilder WithExecute(Action execute, IObservable<bool> canExecute)
        {
            _command = Delegate.Combine(_command, new ActionMapper(execute).Action);

            return WithCanExecute(canExecute);
        }

        public CommandRegistrationBuilder WithExecute(Action execute)
        {
            _command = Delegate.Combine(_command, new ActionMapper(execute).Action);
            return this;
        }

        public CommandRegistrationBuilder WithCanExecute(IEnumerable<IObservable<bool>> canExecute)
        {
            _canExecute.AddRange(canExecute);

            return this;
        }

        public CommandRegistrationBuilder WithCanExecute(IObservable<bool> canExecute)
        {
            _canExecute.Add(canExecute);

            return this;
        }

        public void ThenRegister(string name)
        {
            if (_command == null) return;

            IObservable<bool> canExec;

            switch (_canExecute.Count)
            {
                case 0:
                    canExec = Observable.Return(true);
                    break;
                case 1:
                    canExec = _canExecute[0];
                    break;
                default:
                    canExec = _canExecute.CombineLatest(list => list.All(b => b));
                    break;
            }
            
            _register(name, (Action<object?>) _command, canExec);
        }

        private sealed class ActionMapper
        {
            private readonly Action _action;

            public ActionMapper(Action action)
            {
                _action = action;
            }

            public Action<object?> Action => ActionImpl;

            private void ActionImpl(object? o)
            {
                _action();
            }
        }

        //private sealed class FuncMapper
        //{
        //    private readonly Func<bool> _action;

        //    public FuncMapper(Func<bool> action)
        //    {
        //        _action = action;
        //    }

        //    public Func<object?, bool> Action => ActionImpl;

        //    private bool ActionImpl(object? o)
        //    {
        //        return _action();
        //    }
        //}
    }
}