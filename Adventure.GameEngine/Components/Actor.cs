using EcsRx.Components;
using EcsRx.ReactiveData;
using Newtonsoft.Json;

namespace Adventure.GameEngine.Components
{
    public sealed class Actor : IComponent
    {
        public ReactiveProperty<string> Location { get; }

        public string Name { get; set; } = "Unbekannt";

        [JsonConstructor]
        public Actor(ReactiveProperty<string> location) 
            => Location = location;

        public Actor(string location) 
            => Location = new ReactiveProperty<string>(location);
    }
}