using System;
using Akkatecture.Commands;

namespace TextAdventures.Builder.Data.Command
{
    public interface ICommandMetadata
    {
        string Name { get; set; }
    }

    public interface IGameCommand : ICommand
    {
        Type Target { get; }
    }
}