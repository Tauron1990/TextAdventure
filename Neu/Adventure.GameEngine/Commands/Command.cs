using System;
using System.IO;
using Adventure.GameEngine.Core.Persistence;
using EcsRx.Events;
using JetBrains.Annotations;

namespace Adventure.GameEngine.Commands
{
    public sealed class EventInfo : IPersitable
    {
        public string Name { get; private set; }

        public bool OneTime { get; private set; }

        public bool Triggered { get; set; }

        public EventInfo(string name, bool oneTime)
        {
            Name = name;
            OneTime = oneTime;
        }

        public EventInfo()
        {
            Name = string.Empty;
            OneTime = true;
        }

        void IPersitable.WriteTo(BinaryWriter writer)
        {
            writer.Write(Name);
            writer.Write(OneTime);
            writer.Write(Triggered);
        }

        void IPersitable.ReadFrom(BinaryReader reader)
        {
            Name = reader.ReadString();
            OneTime = reader.ReadBoolean();
            Triggered = reader.ReadBoolean();
        }
    }

    public abstract class Command : IEquatable<Command>, IPersitable
    {
        [PublicAPI]
        public string Id { get; }

        public string? Category { get; set; }

        public EventInfo? TriggersEvent { get; set; }

        public bool Hidden { get; set; }

        public bool HideOnExecute { get; set; }

        protected Command(string id)
            => Id = id;

        void IPersitable.WriteTo(BinaryWriter writer)
        {
            BinaryHelper.WriteNull(Category, writer);
            BinaryHelper.WriteNull(TriggersEvent, writer);
            writer.Write(Hidden);
            writer.Write(HideOnExecute);
        }

        void IPersitable.ReadFrom(BinaryReader reader)
        {
            Category = BinaryHelper.ReadNull(reader);
            TriggersEvent = BinaryHelper.ReadNull(reader, () => new EventInfo());
            Hidden = reader.ReadBoolean();
            HideOnExecute = reader.ReadBoolean();
        }

        protected internal abstract void Dispatch(IEventSystem system);

        public bool Equals(Command? other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return Id == other.Id;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != this.GetType())
                return false;
            return Equals((Command)obj);
        }

        public override int GetHashCode()
            => Id.GetHashCode();

        public static bool operator ==(Command? left, Command? right)
            => Equals(left, right);

        public static bool operator !=(Command? left, Command? right)
            => !Equals(left, right);
    }

    public abstract class Command<TThis> : Command
        where TThis : Command<TThis>
    {


        protected internal override void Dispatch(IEventSystem eventSystem)
            => eventSystem.Publish((TThis)this);



        protected Command(string id)
            : base(id)
        {
        }
    }
}