namespace Adventure.GameEngine.Core.Persistence
{
    public interface IPersistComponent : IPersitable
    {
        public string Id { get; }
    }
}