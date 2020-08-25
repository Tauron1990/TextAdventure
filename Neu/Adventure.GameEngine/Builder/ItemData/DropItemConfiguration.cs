using Adventure.GameEngine.Builder.Core;

namespace Adventure.GameEngine.Builder.ItemData
{
    public sealed class DropItemConfiguration<TEventSource, TItem>
        where  TEventSource : IEntityConfiguration, IWithMetadata
        where TItem : ItemBuilder<TEventSource>
    {
        public TItem Target { get; }

        public DropItemConfiguration(TItem target)
        {
            target.BluePrint.CanDrop = true;
            Target = target;
        }
    }
}