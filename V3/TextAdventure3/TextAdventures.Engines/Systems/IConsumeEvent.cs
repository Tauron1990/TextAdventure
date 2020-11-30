namespace TextAdventures.Engine.Systems
{
    public interface IConsumeEvent<TEvent>
    {
        void Process(TEvent evt);
    }
}