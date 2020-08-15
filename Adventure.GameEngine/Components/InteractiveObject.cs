using Adventure.TextProcessing.Synonyms;
using EcsRx.Components;

namespace Adventure.GameEngine.Components
{
    public sealed class InteractiveObject : IComponent
    {
        public VerbCodes targetAction { get; }

        public string? TriggerEvent { get; set; }

        public InteractiveObject(VerbCodes targetAction, string? triggerEvent)
        {
            this.targetAction = targetAction;
            TriggerEvent = triggerEvent;
        }
    }
}