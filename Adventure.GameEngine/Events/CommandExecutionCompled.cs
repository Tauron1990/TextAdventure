namespace Adventure.GameEngine.Events
{
    public sealed class CommandExecutionCompled
    {
        public string? Result { get; }

        public CommandExecutionCompled(string? result) => Result = result;
    }
}