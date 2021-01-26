﻿using System;
using System.Reflection;
using JetBrains.Annotations;

namespace Tauron.Application.CommonUI.Commands
{
    public sealed class EventData
    {
        public EventData([NotNull] object sender, [NotNull] object eventArgs)
        {
            Sender = Argument.NotNull(sender, nameof(sender));
            EventArgs = Argument.NotNull(eventArgs, nameof(eventArgs));
        }

        [NotNull] public object EventArgs { get; }

        [NotNull] public object Sender { get; }
    }

    [PublicAPI]
    public sealed class MethodCommand : CommandBase
    {
        private readonly MethodInfo _method;
        private readonly MethodType _methodType;

        public MethodCommand(MethodInfo method, object context)
        {
            _method = Argument.NotNull(method, nameof(method));
            Context = Argument.NotNull(context, nameof(context));

            _methodType = (MethodType) method.GetParameters().Length;
            if (_methodType != MethodType.One) return;
            if (_method.GetParameters()[0].ParameterType != typeof(EventData)) _methodType = MethodType.EventArgs;
        }

        private object? Context { get; }

        public override void Execute(object? parameter)
        {
            var temp = parameter as EventData;
            var args = _methodType switch
                       {
                           MethodType.Zero      => Array.Empty<object>(),
                           MethodType.One       => new object?[] {temp},
                           MethodType.Two       => new[] {temp?.Sender, temp?.EventArgs},
                           MethodType.EventArgs => new[] {temp?.EventArgs},
                           _                    => Array.Empty<object>()
                       };

            _method.InvokeFast(Context, args);
        }

        private enum MethodType
        {
            Zero = 0,
            One,
            Two,
            EventArgs
        }
    }
}