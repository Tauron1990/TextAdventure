using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using TextAdventures.Engine.Data;

namespace TextAdventures.Engine.Modules.Command
{
    public record CommandData(string Category, Type TargetType, bool Enable);

    public record CommandEvent(ImmutableDictionary<string, ImmutableList<Type>> Commands);

    public record CommandLayerEvent(CommandLayerKey Name, CommandData Data);

    public record UpdateCommandLayerEvent(ImmutableDictionary<CommandLayerKey, CommandData> Data);

    public sealed class CommandLayerKey
    {
        public CommandLayerKey(string name, int order)
        {
            Name = name;
            Order = order;
        }

        public string Name { get; }

        public int Order { get; }
    }

    public class CommandLayerComponent : ComponentBase
    {
        public ImmutableDictionary<CommandLayerKey, CommandData> CommandLayer
        {
            get => GetData(ImmutableDictionary<CommandLayerKey, CommandData>.Empty);
            set => SetData(value);
        }

        public IEnumerable<CommandData> CommandData
            => CommandLayer.Where(l => l.Value.Enable).OrderBy(p => p.Key.Order).Select(p => p.Value);

        protected internal override void ApplyEvent(object @event)
        {
            CommandLayer = @event switch
            {
                CommandLayerEvent layer => CommandLayer.SetItem(layer.Name, layer.Data),
                _ => CommandLayer
            };
        }
    }
}