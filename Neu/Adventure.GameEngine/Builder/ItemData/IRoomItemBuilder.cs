using Adventure.GameEngine.Builder.Core;

namespace Adventure.GameEngine.Builder.ItemData
{
    public interface IRoomItemBuilder<out TImpl> : IEntityConfiguration, IWithMetadata
    {
        string Id { get; }
        
        TImpl Impl { get; }
    }
}