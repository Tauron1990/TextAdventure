using System.IO;
using Adventure.GameEngine.Core;
using Adventure.GameEngine.Core.Persistence;
using JetBrains.Annotations;

namespace Adventure.GameEngine.Systems.Components
{
    [PublicAPI]
    public sealed class PointOfInterst : IPersitable
    {
        public string? ItemId { get; set; }

        public bool Show { get; set; }

        public LazyString Text { get; set; }
        

        public PointOfInterst(bool show, LazyString text)
        {
            Show = show;
            Text = text;
        }

        public PointOfInterst()
        {
            Show = false;
            Text = LazyString.New(string.Empty);
        }

        void IPersitable.WriteTo(BinaryWriter writer)
        {
            BinaryHelper.WriteNull(ItemId, writer);
            writer.Write(Show);
            BinaryHelper.Write(writer, Text);
        }

        void IPersitable.ReadFrom(BinaryReader reader)
        {
            ItemId = BinaryHelper.ReadNull(reader);
            Show = reader.ReadBoolean();
            Text = BinaryHelper.Read(reader, LazyString.New);
        }
    }
}