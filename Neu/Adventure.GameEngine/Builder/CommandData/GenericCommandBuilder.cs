using Adventure.GameEngine.Core;
using Adventure.GameEngine.Core.Blueprints;
using JetBrains.Annotations;

namespace Adventure.GameEngine.Builder.CommandData
{
    [PublicAPI]
    public sealed class GenericCommandBuilder
    {
        private readonly GenericCommandBlueprint _blueprint;

        public GenericCommandBuilder(CommandId id, GenericCommandBlueprint blueprint)
        {
            Id = id;
            _blueprint = blueprint;
        }

        public CommandId Id { get; }

        public GenericCommandBuilder WithDescription(string descriprion)
            => WithDescription(LazyString.New(descriprion));

        public GenericCommandBuilder WithDescription(LazyString descriprion)
        {
            _blueprint.Description = descriprion;
            return this;
        }
    }
}