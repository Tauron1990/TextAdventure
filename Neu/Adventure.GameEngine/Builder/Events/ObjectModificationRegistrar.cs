using System;
using Adventure.GameEngine.Builder.Core;
using Adventure.GameEngine.Commands;
using Adventure.GameEngine.Core.Blueprints;
using EcsRx.Blueprints;

namespace Adventure.GameEngine.Builder.Events
{
    public class ObjectModificationRegistrar<TEventSource, TObject> : EventRegistrar<TEventSource, TObject> 
        where TEventSource : IWithMetadata, IEntityConfiguration
    {
        private readonly Action<TObject> _changer;

        public ObjectModificationRegistrar(Action<TObject> changer)
            => _changer = changer;

        protected internal override IBlueprint CreateRegistration(TEventSource eventSource, TObject data, string name)
            => new EntityEvent<ObjectChangeCommand>(ChangeAction.WithObjectAction(data, _changer), name);
    }
}