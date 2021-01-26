using System;
using System.Reactive.Linq;
using Akka.Actor;
using JetBrains.Annotations;
using Tauron;
using Tauron.Features;
using TextAdventures.Engine.Data;

namespace TextAdventures.Engine.Actors
{
    [PublicAPI]
    public abstract class GameProcess<TState> : ActorFeatureBase<TState>
    {
        public GameCore Game => Context.System.GetExtension<GameCore>();
        
        protected override void Config()
        {
            SupervisorStrategy = new OneForOneStrategy(_ => Directive.Escalate);

            Receive<PreInitStage>(obs => obs.Select(p =>
                                                    {
                                                        PreInit(p.Event, p.State);
                                                        return p.Event;
                                                    })
                                            .ToSender());

            Receive<LoadingCompled>(obs => obs.Subscribe(LoadingCompled));
        }
        
        protected virtual void PreInit(PreInitStage msg, TState state) { }

        protected virtual void LoadingCompled(StatePair<LoadingCompled, TState> message) { }
        
    }
}