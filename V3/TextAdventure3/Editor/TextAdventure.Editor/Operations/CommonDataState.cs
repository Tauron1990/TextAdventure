using Tauron.Application.Workshop.Mutating;
using Tauron.Application.Workshop.Mutation;
using Tauron.Application.Workshop.StateManagement;
using Tauron.Application.Workshop.StateManagement.Attributes;
using TextAdventure.Editor.Data.PartialData;
using TextAdventure.Editor.Operations.Events;

namespace TextAdventure.Editor.Operations
{
    [State]
    public sealed class CommonDataState : StateBase<CommonData>
    {
        public IEventSource<NameVersionChangedEvent> NameChanged { get; }

        public CommonDataState(ExtendedMutatingEngine<MutatingContext<CommonData>> engine) : base(engine)
        {
            NameChanged = engine.EventSource<CommonData, NameVersionChangedEvent>();
        }
    }
}