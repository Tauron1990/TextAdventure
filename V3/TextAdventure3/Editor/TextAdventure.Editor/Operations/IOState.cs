using System;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using Tauron;
using Tauron.Application.Workshop.Mutating;
using Tauron.Application.Workshop.Mutation;
using Tauron.Application.Workshop.StateManagement;
using Tauron.Application.Workshop.StateManagement.Attributes;
using TextAdventure.Editor.Data.PartialData;
using TextAdventure.Editor.Data.ProjectData;
using TextAdventure.Editor.Operations.Events;

namespace TextAdventure.Editor.Operations
{
    [State]
    public sealed class IOState : StateBase<IOData>
    {
        public IOState(ExtendedMutatingEngine<MutatingContext<IOData>> engine)
            : base(engine)
        {
            ProjectLoaded = engine.EventSource<IOData, ProjectLoadedEvent>();
            ProjectLoadFailed = engine.EventSource<IOData, LoadFailedEvent>();
            ProjectSaved = engine.EventSource<IOData, ProjectSaveEvent>();
        }

        public IEventSource<ProjectLoadedEvent> ProjectLoaded { get; }

        public IEventSource<LoadFailedEvent> ProjectLoadFailed { get; }

        public IEventSource<ProjectSaveEvent> ProjectSaved { get; }

        public IObservable<GameProject> RawProject 
            => Query(EmptyQuery.Instance)
              .ToObservable()
              .NotNull()
              .Select(d => d.Project);
    }
}