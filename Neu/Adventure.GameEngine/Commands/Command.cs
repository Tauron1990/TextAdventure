using System;
using EcsRx.Events;
using JetBrains.Annotations;

namespace Adventure.GameEngine.Commands
{
    public abstract class Command<TThis> : IEquatable<Command<TThis>>
        where TThis : Command<TThis>
    {
        [PublicAPI]
        public string Id { get; }

        protected Command(string id)
            => Id = id;

        internal void Dispatch(IEventSystem eventSystem)
            => eventSystem.Publish((TThis)this);

        public bool Equals(Command<TThis>? other)
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
            return Equals((Command<TThis>) obj);
        }

        public override int GetHashCode()
            => Id.GetHashCode();

        public static bool operator ==(Command<TThis>? left, Command<TThis>? right)
            => Equals(left, right);

        public static bool operator !=(Command<TThis>? left, Command<TThis>? right)
            => !Equals(left, right);
    }
}