using System;
using Tauron.Application.Workshop.Mutating.Changes;
using Tauron.Application.Workshop.StateManagement;
using TextAdventure.Editor.Data.ProjectData;
using TextAdventure.Editor.Operations.Events;

namespace TextAdventure.Editor.Data.PartialData
{
    public record IOData(GameProject Project, string SourcePath, bool IsDummy) : IStateEntity, ICanApplyChange<IOData>
    {
        public IOData()
            : this(GameProject.Create("Dummy", new Version(0,0)), string.Empty, true)
        {
            
        }

        public string Id => Project.GameName;


        public IOData Apply(MutatingChange apply)
        {
            return apply switch
            {
                ProjectLoadedEvent l => this with{ IsDummy = false, SourcePath = l.SourcePath, Project = l.Project},
                _ => this
            };
        }
    }
}