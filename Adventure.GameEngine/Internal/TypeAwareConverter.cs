using System;
using System.Collections.Generic;
using EcsRx.Components;
using Newtonsoft.Json;

namespace Adventure.GameEngine.Internal
{
    public sealed class TypeAwareConverter : JsonConverter<List<IComponent>>
    {
        public override void WriteJson(JsonWriter writer, List<IComponent> value, JsonSerializer serializer)
        {
            writer.WriteStartObject();

            var count = value.Count;
            var entry = 0;

            writer.WritePropertyName("DataCount");
            writer.WriteValue(count);

            foreach (var component in value)
            {
                writer.WritePropertyName("Type" + entry);
                writer.WriteValue(component.GetType().AssemblyQualifiedName);

                writer.WritePropertyName("ObjectData" + entry);
                serializer.Serialize(writer, component);
                
                entry++;
            }

            writer.WriteEndObject();
        }

        public override List<IComponent> ReadJson(JsonReader reader, Type objectType, List<IComponent> existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            List<IComponent> components = new List<IComponent>();

            ReadProperty(reader, "DataCount");

            var count = reader.ReadAsInt32();
            if (count == null)
                ThrowException();
            var entry = 0;

            for (int i = 0; i < count!.Value; i++)
            {
                ReadProperty(reader, "Type" + entry);
                var targetType = Type.GetType(reader.ReadAsString() ?? string.Empty, true);

                ReadProperty(reader, "ObjectData" + entry);

                Read(reader);
                var comp = serializer.Deserialize(reader, targetType) as IComponent;

                if (comp == null)
                    ThrowException();

                components.Add(comp!);
                entry++;
            }

            Read(reader);
            return components;
        }

        private void ReadProperty(JsonReader reader, string expected)
        {
            Read(reader);
            if (reader.TokenType != JsonToken.PropertyName || reader.Value?.ToString() != expected)
                ThrowException();
        }

        private void Read(JsonReader reader)
        {
            if (!reader.Read()) ThrowException();
        }

        private void ThrowException()
        {
            throw new InvalidOperationException("TypeAwareConverter Invalid Structure");
        }
    }
}