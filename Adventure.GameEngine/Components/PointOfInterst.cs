using Adventure.GameEngine.Core;
using Newtonsoft.Json;

namespace Adventure.GameEngine.Components
{
    public sealed class PointOfInterst
    {
        public string Id { get; }

        public bool Show { get; set; }

        public LazyString Text { get; set; }

        [JsonConstructor]
        public PointOfInterst(string id, bool show, LazyString text)
        {
            Id = id;
            Show = show;
            Text = text;
        }
    }
}