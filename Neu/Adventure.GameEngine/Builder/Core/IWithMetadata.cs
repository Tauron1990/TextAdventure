using System.Collections.Generic;
using System.Collections.Immutable;

namespace Adventure.GameEngine.Builder.Core
{
    public interface IWithMetadata
    {
        Dictionary<string, object> Metadata { get; }
    }
}