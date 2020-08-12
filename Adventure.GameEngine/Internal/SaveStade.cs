using System.Collections.Generic;
using Newtonsoft.Json;

namespace Adventure.GameEngine.Internal
{
    public sealed class SaveStade
    {
        public List<EntityCollectionData> Collections { get; }

        [JsonConstructor]
        public SaveStade(List<EntityCollectionData> collections) 
            => Collections = collections;

        public SaveStade() => Collections = new List<EntityCollectionData>();
    }
}