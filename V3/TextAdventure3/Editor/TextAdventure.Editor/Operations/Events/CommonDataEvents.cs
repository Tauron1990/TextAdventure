using System;
using Tauron.Application.Workshop.Mutating.Changes;

namespace TextAdventure.Editor.Operations.Events
{
    public sealed record NameVersionChangedEvent(bool HasChanged, string NewName, Version NewVersion) : MutatingChange;
}