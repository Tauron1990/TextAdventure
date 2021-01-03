using Tauron.Application.Workshop.StateManagement;
using Tauron.Application.Workshop.StateManagement.Attributes;
using TextAdventure.Editor.Operations.Command;

namespace TextAdventure.Editor.Operations
{
    [Effect]
    public sealed class SaveInvoker : IEffect
    {
        public void Handle(IStateAction action, IActionInvoker dispatcher) { dispatcher.Run(new SaveCommand()); }
        public bool ShouldReactToAction(IStateAction action) => action is ISaveRelevant;
    }
}