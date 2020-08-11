using EcsRx.Components;
using EcsRx.Entities;

namespace TextAdventure
{
    public sealed class Test : IComponent
    {
        public string Value { get; set; }

        public Test(string value) => Value = value;

        public Test()
        {
            
        }
    }
}