using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using FastExpressionCompiler;
using JetBrains.Annotations;

namespace Tauron.Application
{
    [PublicAPI]
    public static class ObservablePropertyChangedExtensions
    {
        public static IObservable<TProp> WhenAny<TProp>(this IObservablePropertyChanged @this,
            Expression<Func<TProp>> prop)
        {
            var name = Reflex.PropertyName(prop);
            var func = prop.CompileFast();

            return @this.PropertyChangedObservable.Where(s => s == name).Select(_ => func());
        }
    }

    [PublicAPI]
    public interface IObservablePropertyChanged
    {
        IObservable<string> PropertyChangedObservable { get; }
    }

    [Serializable]
    [PublicAPI]
    [DebuggerStepThrough]
    public abstract class ObservableObject : INotifyPropertyChangedMethod, IObservablePropertyChanged
    {
        private readonly Subject<string> _propertyChnaged = new();
        private ConcurrentDictionary<string, object?>? _store;

        public event PropertyChangedEventHandler? PropertyChanged;

        [NotifyPropertyChangedInvocator]
        public virtual void OnPropertyChanged([CallerMemberName] string? eventArgs = null)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(Argument.NotNull(eventArgs!, nameof(eventArgs))));
        }

        public IObservable<string> PropertyChangedObservable => _propertyChnaged.AsObservable();

        public void SetProperty<TType>(ref TType property, TType value, [CallerMemberName] string? name = null)
        {
            if (EqualityComparer<TType>.Default.Equals(property, value)) return;

            property = value;
            OnPropertyChangedExplicit(Argument.NotNull(name!, nameof(name)));
        }

        public void SetProperty<TType>(ref TType property, TType value, Action changed,
            [CallerMemberName] string? name = null)
        {
            if (EqualityComparer<TType>.Default.Equals(property, value)) return;

            property = value;
            OnPropertyChangedExplicit(Argument.NotNull(name!, nameof(name)));
            changed();
        }

        public void SetProperty<TType>(TType value, [CallerMemberName] string? name = null)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            _store ??= new ConcurrentDictionary<string, object?>();

            if (EqualityComparer<TType>.Default.Equals(GetProperty<TType>(name), value)) return;

            _store[name] = value;
            OnPropertyChangedExplicit(Argument.NotNull(name!, nameof(name)));
        }

        public void SetProperty<TType>(TType value, Action changed, [CallerMemberName] string? name = null)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            _store ??= new ConcurrentDictionary<string, object?>();

            if (EqualityComparer<TType>.Default.Equals(GetProperty<TType>(name), value)) return;

            _store[name] = value;
            OnPropertyChangedExplicit(Argument.NotNull(name!, nameof(name)));
            changed();
        }

        public TType? GetProperty<TType>([CallerMemberName] string? name = null)
        {
            _store ??= new ConcurrentDictionary<string, object?>();

            #pragma warning disable CS8620 // Das Argument kann aufgrund von Unterschieden bei der NULL-Zulässigkeit von Verweistypen nicht für den Parameter verwendet werden.
            return _store.GetValueOrDefault(name) switch
                #pragma warning restore CS8620 // Das Argument kann aufgrund von Unterschieden bei der NULL-Zulässigkeit von Verweistypen nicht für den Parameter verwendet werden.
                {
                    TType value => value,
                    _ => default
                };
        }

        public virtual void OnPropertyChanged(PropertyChangedEventArgs eventArgs)
        {
            OnPropertyChanged(this, Argument.NotNull(eventArgs, nameof(eventArgs)));
        }

        public virtual void OnPropertyChanged(object sender, PropertyChangedEventArgs eventArgs)
        {
            if (!string.IsNullOrWhiteSpace(eventArgs.PropertyName))
                _propertyChnaged.OnNext(eventArgs.PropertyName);
            PropertyChanged?.Invoke(Argument.NotNull(sender, nameof(sender)),
                Argument.NotNull(eventArgs, nameof(eventArgs)));
        }


        public virtual void OnPropertyChanged<T>(Expression<Func<T>> eventArgs)
        {
            OnPropertyChanged(
                new PropertyChangedEventArgs(Reflex.PropertyName(Argument.NotNull(eventArgs, nameof(eventArgs)))));
        }


        public virtual void OnPropertyChangedExplicit(string propertyName)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(Argument.NotNull(propertyName, nameof(propertyName))));
        }
    }
}