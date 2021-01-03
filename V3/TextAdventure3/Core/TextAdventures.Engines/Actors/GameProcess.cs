using Akka.Actor;
using JetBrains.Annotations;
using TextAdventures.Engine.Data;

namespace TextAdventures.Engine.Actors
{
    [PublicAPI]
    public abstract class GameProcess : ReceiveActor
    {
        public GameCore Game => Context.System.GetExtension<GameCore>();

        protected GameProcess()
        {
            Receive<LoadingCompled>(LoadingCompled);
            Receive<PreInitStage>(PreInitHandler);
        }

        private void PreInitHandler(PreInitStage obj)
        {
            try
            {
                PreInit(obj);
            }
            finally
            {
                Sender.Tell(obj);
            }
        }

        protected virtual void PreInit(PreInitStage msg) { }

        protected virtual void LoadingCompled(LoadingCompled obj) { }

        protected override SupervisorStrategy SupervisorStrategy()
            => new OneForOneStrategy(_ => Directive.Escalate);
    }
}