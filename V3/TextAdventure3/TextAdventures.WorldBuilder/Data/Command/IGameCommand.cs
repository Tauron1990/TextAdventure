using System;
using Akkatecture.Commands;

namespace TextAdventures.Builder.Data.Command
{
    public interface IGameCommand : ICommand
    {
        Type Target { get; }
    }
}