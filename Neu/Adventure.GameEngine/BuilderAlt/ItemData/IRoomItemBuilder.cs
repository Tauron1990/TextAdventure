namespace Adventure.GameEngine.BuilderAlt.ItemData
{
    public interface IRoomItemBuilder<out TImpl>
    {
        string Id { get; }

        RoomBuilder Room { get; }

        TImpl Impl { get; }
    }
}