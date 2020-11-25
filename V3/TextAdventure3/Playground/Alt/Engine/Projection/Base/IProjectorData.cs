namespace TextAdventures.Engine.Projection.Base
{
    public interface IProjectorData<TKey>
    {
        public TKey Id { get; set; }
    }
}