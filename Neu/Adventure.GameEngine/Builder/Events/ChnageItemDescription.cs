using Adventure.GameEngine.Builder.Core;
using Adventure.GameEngine.Commands;
using Adventure.GameEngine.Core;
using Adventure.GameEngine.Core.Blueprints;
using EcsRx.Blueprints;

namespace Adventure.GameEngine.Builder.Events
{
    public sealed class ChnageItemDescription<TReturn> : EventRegistrar<TReturn, ItemBluePrint> where TReturn : IWithMetadata, IEntityConfiguration
    {
        private readonly LazyString _description;

        public ChnageItemDescription(LazyString description) =>
            _description = description;

        protected internal override IBlueprint CreateRegistration(TReturn eventSource, ItemBluePrint data, string name) =>
            new EntityEvent<ChangeItemCommand>(ChangeAction.WithItemDescription(data.Id, _description), name);
    }
}