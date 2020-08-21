using System.IO;
using Adventure.GameEngine.Core;
using Adventure.GameEngine.Core.Persistence;
using JetBrains.Annotations;

namespace Adventure.GameEngine.Systems.Components
{
    [PublicAPI]
    public sealed class PointOfInterst : IPersitable
    {
        public string Id { get; private set; }

        public bool Show { get; set; }

        public LazyString Text { get; set; }
        

        public PointOfInterst(string id, bool show, LazyString text)
        {
            Id = id;
            Show = show;
            Text = text;
        }

        public PointOfInterst()
        {
            Id = string.Empty;
            Show = false;
            Text = new LazyString(string.Empty);
        }

        void IPersitable.WriteTo(BinaryWriter writer)
        {
            writer.Write(Id);
            writer.Write(Show);
            BinaryHelper.Write(writer, Text);
        }

        void IPersitable.ReadFrom(BinaryReader reader)
        {
            Id = reader.ReadString();
            Show = reader.ReadBoolean();
            Text = BinaryHelper.Read<LazyString>(reader);
        }
    }
}