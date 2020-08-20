using System.IO;
using Adventure.GameEngine.Persistence;
using Adventure.TextProcessing.Synonyms;
using EcsRx.Components;

namespace Adventure.GameEngine.Components
{
    public sealed class InteractiveObject : IComponent, IPersistComponent
    {
        public InteractiveObject(VerbCodes targetAction, string? triggerEvent)
        {
            TargetAction = targetAction;
            TriggerEvent = triggerEvent;
        }

        public VerbCodes TargetAction { get; }

        public string? TriggerEvent { get; set; }
        void IPersitable.WriteTo(BinaryWriter writer)
            => BinaryHelper.WriteNull(TriggerEvent, writer);

        void IPersitable.ReadFrom(BinaryReader reader)
            => TriggerEvent = BinaryHelper.ReadNull(reader, r => r.ReadString());

        string IPersistComponent.Id => "InteractiveObject";
    }
}