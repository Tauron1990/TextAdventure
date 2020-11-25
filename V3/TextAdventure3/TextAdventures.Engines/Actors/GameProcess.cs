using Akka.Actor;

namespace TextAdventures.Engine.Actors
{
    public abstract class GameProcess : ReceiveActor
    {
        protected override SupervisorStrategy SupervisorStrategy() 
            => new OneForOneStrategy(_ => Directive.Escalate);
    }
}