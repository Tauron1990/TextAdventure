using System.IO;
using Adventure.GameEngine.Core.Persistence;
using EcsRx.Components;

namespace Adventure.GameEngine.Systems.Components
{
    public sealed class InteractiveObject : IComponent, IPersistComponent
    {
        public InteractiveObject(string targetCommand, string? triggerEvent)
        {
            TargetCommand = targetCommand;
            TriggerEvent = triggerEvent;
        }

        public string TargetCommand { get; }

        public string? TriggerEvent { get; set; }

        void IPersitable.WriteTo(BinaryWriter writer)
            => BinaryHelper.WriteNull(TriggerEvent, writer);

        void IPersitable.ReadFrom(BinaryReader reader)
            => TriggerEvent = BinaryHelper.ReadNull(reader, r => r.ReadString());

        string IPersistComponent.Id => "InteractiveObject";
    }
}