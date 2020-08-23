using System.Collections.Generic;

namespace Adventure.Ui.CommandDisplay
{
    public sealed class CommandCategory : ICommandContent
    {
        public string Name { get; }

        public List<ICommandContent> Commands { get; } = new List<ICommandContent>();

        public CommandCategory(string name)
            => Name = name;
    }
}