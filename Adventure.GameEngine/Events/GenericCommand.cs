using System;
using Adventure.GameEngine.Core;
using Adventure.TextProcessing.Interfaces;

namespace Adventure.GameEngine.Events
{
    public sealed class GenericCommand
    {
        public ICommand Command { get; }

        public LazyString? Result { get; set; }

        public GenericCommand(ICommand command)
        {
            Command = command;
        }
    }
}