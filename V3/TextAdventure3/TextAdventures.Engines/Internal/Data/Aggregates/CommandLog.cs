using Akkatecture.Aggregates;
using Akkatecture.Core;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace TextAdventures.Engine.Internal.Data.Aggregates
{
    public sealed class CommandLogId : Identity<CommandLogId>
    {
        [JsonConstructor]
        public CommandLogId(string value) 
            : base(value)
        {
        }

        public CommandLogId()
            : this("commandlog-12C06230-00A4-45C6-AB6D-A151C7C5004B")
        { }
    }

    public sealed class CommandLogStade : GameState<CommandLogStade, CommandLog, CommandLogId>
    {
        public override void Hydrate(CommandLogStade aggregateSnapshot)
        {
            
        }
    }

    public sealed class CommandLog : IAggregateRoot<CommandLogId>
    {
        
    }
}