namespace TextAdventures.Engine.Internal.Data.Projection
{
    public interface IProjectorData<TKey>
    {
        public TKey Id { get; set; }
    }
}