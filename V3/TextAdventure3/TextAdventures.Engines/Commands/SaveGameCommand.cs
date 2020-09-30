namespace TextAdventures.Engine.Commands
{
    public sealed class SaveGameCommand
    {
        public string? Name { get; }

        public SaveGameCommand(string? name) => Name = name;
    }
}