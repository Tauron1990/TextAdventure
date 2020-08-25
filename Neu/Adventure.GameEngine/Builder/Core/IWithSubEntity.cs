using System.Collections.Immutable;
using Adventure.GameEngine.Core.Blueprints;

namespace Adventure.GameEngine.Builder.Core
{
    public interface IWithSubEntity
    {
        ImmutableDictionary<string, IBluePrintProvider> SubEntities { get; }

        void AddToSubSubEntity(string name, IBluePrintProvider blueprint);
    }
}