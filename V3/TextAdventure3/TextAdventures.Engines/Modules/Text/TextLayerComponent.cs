using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using TextAdventures.Engine.Data;

namespace TextAdventures.Engine.Modules.Text
{
    public record TextData(string? Title, string? MainDescription, string? Detail);

    public record TextEvent(string Title, string MainDescription, string Detail);

    public record TextLayerEvent(TextLayerKey Name, TextData Data);

    public record UpdateTextLayerEvent(TextLayerKey Name, TextData Data);

    public sealed class TextLayerKey
    {
        public string Name { get; }

        public int Order { get; }

        public TextLayerKey(string name, int order)
        {
            Name = name;
            Order = order;
        }
    }

    public class TextLayerComponent : ComponentBase
    {
        public ImmutableDictionary<TextLayerKey, TextData> TextLayer
        {
            get => GetData(ImmutableDictionary<TextLayerKey, TextData>.Empty);
            set => SetData(value);
        }

        public IEnumerable<TextData> TextData => TextLayer.OrderBy(p => p.Key.Order).Select(p => p.Value);

        protected internal override void ApplyEvent(object @event)
        {
            TextLayer = @event switch
            {
                TextLayerEvent layer => TextLayer.SetItem(layer.Name, layer.Data),
                _ => TextLayer
            };
        }
    }
}