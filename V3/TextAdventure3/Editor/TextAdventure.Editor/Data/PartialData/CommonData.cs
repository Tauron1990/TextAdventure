using System;
using Tauron.Application.Workshop.Mutating.Changes;
using Tauron.Application.Workshop.StateManagement;
using TextAdventure.Editor.Operations.Events;

namespace TextAdventure.Editor.Data.PartialData
{
    public sealed record CommonData(string Name, Version Version) : IStateEntity, ICanApplyChange<CommonData>
    {
        public string Id => Name;

        public CommonData Apply(MutatingChange apply)
        {
            return apply switch
            {
                NameVersionChangedEvent {HasChanged: true} name => this with {Name = name.NewName, Version = name.NewVersion},
                _ => this
            };
        }
    }
}