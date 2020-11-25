using System;
using Akkatecture.Commands;

namespace TextAdventures.Builder.Commands
{
    public interface ICommandMetadata
    {
        CommandCallData CallData { get; set; }

        string Name { get; set; }
    }

    public interface IGameCommand : ICommand
    {
        Type Target { get; }
    }
}