using Adventure.GameEngine.Core;
using Adventure.GameEngine.Core.Blueprints;
using JetBrains.Annotations;

namespace Adventure.GameEngine.Builder.CommandData
{
    [PublicAPI]
    public sealed class GenericCommandBuilder
    {
        private readonly GenericCommandBlueprint _blueprint;

        public GenericCommandBuilder(GenericCommandBlueprint blueprint)
            => _blueprint = blueprint;

        public GenericCommandBuilder WithDescription(LazyString descriprion)
        {
            _blueprint.Description = descriprion;
            return this;
        }
    }
}