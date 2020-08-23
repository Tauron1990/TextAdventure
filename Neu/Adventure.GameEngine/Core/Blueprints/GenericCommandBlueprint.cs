using Adventure.GameEngine.Commands;
using Adventure.GameEngine.Systems.Components;
using EcsRx.Blueprints;
using EcsRx.Entities;
using EcsRx.Extensions;

namespace Adventure.GameEngine.Core.Blueprints
{
    public sealed class GenericCommandBlueprint : IBlueprint
    {
        private readonly Command _command;
        private readonly string _name;
        public LazyString? Description { get; set; }

        public GenericCommandBlueprint(Command command, string name)
        {
            _command = command;
            _name = name;
        }

        public void Apply(IEntity entity)
        {
            entity.AddComponents(new GenericCommandDescription(_name, Description ?? LazyString.New()), new GenericCommand(_command));
        }
    }
}