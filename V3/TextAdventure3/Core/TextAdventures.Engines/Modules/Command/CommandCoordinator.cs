using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Linq;
using Tauron.Features;
using TextAdventures.Engine.Actors;
using TextAdventures.Engine.Systems;

namespace TextAdventures.Engine.Modules.Command
{
    public sealed class CommandCoordinator : CoordinatorProcess<EmptyState>, IConsumeEvent<UpdateCommandLayerEvent, EmptyState>
    {
        public static IPreparedFeature Prefab()
            => Feature.Create(() => new CommandCoordinator());

        //    .SelectMany(p => p.Data.Select(data => (data, p.CommandData)))
        //.Select(p => (Event: new CommandLayerEvent(p.data.Key, p.data.Value), p.CommandData))

        public IObservable<EmptyState> Process(IObservable<StatePair<UpdateCommandLayerEvent, EmptyState>> obs)
            => EmitEvents(obs.SelectMany(async p => (p.Event.Data, CommandData: await GetGlobalComponent<CommandLayerComponent>())),
                   dat => dat.Data.Select(pair => new CommandLayerEvent(pair.Key, pair.Value)),
                   dat => dat.CommandData)
              .Do(_ => SentUpdate())
              .Select(_ => EmptyState.Inst);

        protected override void LoadingCompled(StatePair<LoadingCompled, EmptyState> obj)
        {
            SentUpdate();
            base.LoadingCompled(obj);
        }

        private void SentUpdate()
            => EmitEvents(
                GetGlobalComponent<CommandLayerComponent>()
                   .SelectMany(i => i.CommandData)
                   .GroupBy(d => d.Category)
                   .Aggregate(ImmutableDictionary<string, ImmutableList<Type>>.Empty,
                        (dictionary, observable) =>
                            dictionary.Add(observable.Key,
                                ImmutableList<Type>.Empty.AddRange(observable.Select(cd => cd.TargetType).ToEnumerable())))
            );
    }
}