using System;
using EcsRx.Events;
using JetBrains.Annotations;

namespace Adventure.GameEngine.Commands
{
    public abstract class Command : IEquatable<Command>
    {
        [PublicAPI]
        public string Id { get; }

        protected Command(string id)
            => Id = id;

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