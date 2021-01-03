﻿using System;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Reactive.Disposables;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Tauron.Akka;

namespace Tauron.Application.Settings
{
    [PublicAPI]
    public abstract class ConfigurationBase : ObservableObject
    {
        private readonly IDefaultActorRef<SettingsManager> _actor;
        private readonly Task _loader;
        private readonly string _scope;

        private ImmutableDictionary<string, string> _dic = ImmutableDictionary<string, string>.Empty;

        private bool _isBlocked;

        protected ConfigurationBase(IDefaultActorRef<SettingsManager> actor, string scope)
        {
            _actor = actor;

            if (actor is EmptyActor<SettingsManager>)
            {
                _scope = string.Empty;
                _loader = Task.CompletedTask;
            }
            else
            {
                _scope = scope;
                _loader = Task.Run(async () => await LoadValues());
            }
        }

        public IDisposable BlockSet()
        {
            _isBlocked = true;
            return Disposable.Create(() => _isBlocked = false);
        }

        private async Task LoadValues()
        {
            var result = await _actor.Ask<ImmutableDictionary<string, string>>(new RequestAllValues(_scope));

            Interlocked.Exchange(ref _dic, result);
        }

        [return: MaybeNull]
        protected TValue GetValue<TValue>(Func<string, TValue> converter, TValue? defaultValue = default, [CallerMemberName] string? name = null)
        {
            try
            {
                _loader.Wait();
                if (string.IsNullOrEmpty(name)) return default!;

                return _dic.TryGetValue(name, out var value) ? converter(value) : defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }

        protected void SetValue(string value, [CallerMemberName] string? name = null)
        {
            if (_isBlocked)
                return;

            if (string.IsNullOrEmpty(name)) return;

            ImmutableInterlocked.AddOrUpdate(ref _dic, name, value, (_, _) => value);

            _actor.Tell(new SetSettingValue(_scope, name, value));

            OnPropertyChanged(name);
        }
    }
}