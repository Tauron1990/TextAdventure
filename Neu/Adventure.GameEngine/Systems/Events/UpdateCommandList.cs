using System.Collections.Generic;
using Adventure.GameEngine.Commands;
using Adventure.GameEngine.Core;

namespace Adventure.GameEngine.Systems.Events
{
    public sealed class UpdateCommandList
    {
        public IEnumerable<(LazyString Name, Command command)> Commands { get; }


        public UpdateCommandList(IEnumerable<(LazyString Name, Command command)> commands)
            => Commands = commands;
    }
}