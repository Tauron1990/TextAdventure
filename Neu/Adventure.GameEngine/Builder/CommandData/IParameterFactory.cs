namespace Adventure.GameEngine.Builder.CommandData
{
    public interface IParameterFactory
    {
        object? Create(ICommandProperties properties);
    }
}