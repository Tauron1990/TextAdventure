using System;

namespace Adventure.GameEngine.Commands
{
    public abstract class ObjectChangeCommand : Command<ObjectChangeCommand>
    {
        public ObjectChangeCommand()
            : base(nameof(ObjectChangeCommand))
        {
            
        }

        public abstract void Run();
    }

    public sealed class ObjectChangeCommand<TObject> : ObjectChangeCommand
    {
        private readonly Action<TObject> _changer;

        private readonly TObject _target;

        public ObjectChangeCommand(Action<TObject> changer, TObject target)
        {
            _changer = changer;
            _target = target;
        }

        public override void Run() 
            => _changer(_target);
    }
}