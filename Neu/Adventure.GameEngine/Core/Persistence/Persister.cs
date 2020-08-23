using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EcsRx.Collections.Database;
using EcsRx.Collections.Entity;
using EcsRx.Entities;
using EcsRx.Extensions;
using JetBrains.Annotations;

namespace Adventure.GameEngine.Core.Persistence
{
    [PublicAPI]
    public sealed class Persister
    {
        private readonly IEntityDatabase _database;

        public Persister(IEntityDatabase database) 
            => _database = database;

        public void Save(string file)
        {
            using var stream = new FileStream(file, FileMode.Create);
            using var writer = new BinaryWriter(stream);

            writer.Write(_database.Collections.Count);
            foreach (var collection in _database.Collections)
            {
                var entities = collection
                    .Where(e => e.HasComponent(typeof(Persist)))
                    .ToArray();

                writer.Write(collection.Id);
                writer.Write(entities.Length);

                var entChack = new HashSet<string>();

                foreach (var entity in entities)
                {
                    var entName = entity.GetComponent<Persist>().Name;
                    if(!entChack.Add(entName))
                        throw new InvalidOperationException($"Duplicate Entity {entName}");

                    writer.Write(entName);

                    var components = entity.Components
                        .OfType<IPersistComponent>().ToArray();

                    var compCheck = new HashSet<string>();

                    writer.Write(components.Length);
                    foreach (var component in components)
                    {
                        if(!compCheck.Add(component.Id))
                            throw new InvalidOperationException($"Duplicate Component {entName} -- {component.Id}");

                        writer.Write(component.Id);
                        component.WriteTo(writer);
                    }
                }
            }
        }

        public void Load(string file, Func<string, IEntityCollection, IEntity> entityFactory)
        {
            using var stream = new FileStream(file, FileMode.Open);
            using var reader = new BinaryReader(stream);

            var collectionCount = reader.ReadInt32();
            for (var i = 0; i < collectionCount; i++)
            {
                var collection = _database.GetCollection(reader.ReadInt32());
                var entityCount = reader.ReadInt32();

                for (var j = 0; j < entityCount; j++)
                {
                    string name = reader.ReadString();
                    var target = collection.Where(e => e.HasComponent<Persist>())
                        .FirstOrDefault(e => e.GetComponent<Persist>().Name == name) 
                                 ?? entityFactory(name, collection);

                    var componentCount = reader.ReadInt32();

                    for (var k = 0; k < componentCount; k++)
                    {
                        var componentName = reader.ReadString();
                        var targetComponent =
                            target.Components.Single(c => c is IPersistComponent cc 
                                                          && cc.Id == componentName);

                        ((IPersistComponent)targetComponent).ReadFrom(reader);
                    }
                }
            }
        }
    }
}