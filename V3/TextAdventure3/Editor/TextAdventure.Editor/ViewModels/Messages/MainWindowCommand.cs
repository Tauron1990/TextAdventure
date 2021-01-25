namespace TextAdventure.Editor.ViewModels.Messages
{
    public record MainWindowCommand(MainWindowCommand.CommandType Command)
    {
        public enum CommandType
        {
            Open,
            New
        }

        public static MainWindowCommand Open()
            => new(CommandType.Open);

        public static MainWindowCommand New()
            => new(CommandType.New);
    }
}