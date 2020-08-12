using System.Collections.Immutable;
using EcsRx.Components;
using Newtonsoft.Json;

namespace Adventure.GameEngine.Components
{
    public sealed class ReplayInfo : IComponent
    {
        public ImmutableList<string> Commands { get; private set; }

        [JsonConstructor]
        public ReplayInfo(ImmutableList<string> commands) => Commands = commands;

        public ReplayInfo() => Commands = ImmutableList<string>.Empty;

        public void Add(string command)
        {
            Commands = Commands.Add(command);
        }
    }
}