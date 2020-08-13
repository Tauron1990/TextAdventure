using EcsRx.Components;
using Newtonsoft.Json;

namespace Adventure.GameEngine.Components
{
    public sealed class GenericCommandDescription : IComponent
    {
        public string Name { get; }

        public GenericCommandDescription()
        {
            
        }

        [JsonConstructor]
        public GenericCommandDescription(string name)
        {
            Name = name;
        }
    }
}