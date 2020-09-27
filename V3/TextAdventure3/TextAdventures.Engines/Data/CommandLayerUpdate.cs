using Akkatecture.ValueObjects;

namespace TextAdventures.Engine.Data
{
    public enum CommandLayerUpdateValue
    {
        Replace,
        Remove,
        Add
    }

    public sealed class CommandLayerUpdate : SingleValueObject<CommandLayerUpdateValue>
    {
        public CommandLayerUpdate(CommandLayerUpdateValue value) : base(value)
        {
        }
    }
}