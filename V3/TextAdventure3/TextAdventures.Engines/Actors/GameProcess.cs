using Akka.Actor;

namespace TextAdventures.Engine.Actors
{
    public abstract class GameProcess : ReceiveActor
    {
        protected GameProcess()
        {
            Receive<LoadingCompled>(LoadingCompled);
        }

        protected virtual void LoadingCompled(LoadingCompled obj)
        {
            
        }

        protected override SupervisorStrategy SupervisorStrategy() 
            => new OneForOneStrategy(_ => Directive.Escalate);
    }
}