namespace Adventure.GameEngine.Builder.ItemData
{
    public interface IRoomItemBuilder<out TImpl>
    {
        string Id { get; }

        RoomBuilder Room { get; }

        TImpl Impl { get; }
    }
}