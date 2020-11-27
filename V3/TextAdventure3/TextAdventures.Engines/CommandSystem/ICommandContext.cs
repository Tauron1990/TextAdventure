using JetBrains.Annotations;
using TextAdventures.Engine.Data;

namespace TextAdventures.Engine.CommandSystem
{
    [PublicAPI]
    public interface ICommandContext<out TComponent>
    {
        GameCore Game { get; }

        TComponent Component { get; }

        GameObject Object { get; }
    }
}