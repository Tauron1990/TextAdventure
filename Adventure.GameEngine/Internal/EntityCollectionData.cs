using System.Collections.Generic;
using Newtonsoft.Json;

namespace Adventure.GameEngine.Internal
{
    public sealed class EntityCollectionData
    {
        public int Id { get; }

        public List<EntityData> Entitys { get; }

        [JsonConstructor]
        public EntityCollectionData(int id, List<EntityData> entitys)
        {
            Id = id;
            Entitys = entitys;
        }

        public EntityCollectionData(int id)
        {
            Id = id;
            Entitys = new List<EntityData>();
        }
    }
}