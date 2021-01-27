using System;
using System.Reactive.Linq;
using Tauron;
using Tauron.Akka;
using Tauron.Features;
using TextAdventures.Engine.Actors;
using TextAdventures.Engine.Systems;

namespace TextAdventures.Engine.Modules.Text
{
    public sealed class TextCoordinator : CoordinatorProcess<EmptyState>, IConsumeEvent<UpdateTextLayerEvent, EmptyState>
    {
        public static IPreparedFeature Prefab() => Feature.Create(() => new TextCoordinator());

        public IObservable<EmptyState> Process(IObservable<StatePair<UpdateTextLayerEvent, EmptyState>> obs)
        {
            return obs
                  .SelectMany(p => GetGlobalComponent<TextLayerComponent>().Select(c => (c, p)))
                  .Select(evt =>
                          {
                              var (textComponent, ((name, textData), state, _)) = evt;
                              EmitEvents(textComponent, new TextLayerEvent(name, textData));
                              SentUpdate();

                              return state;
                          });
        }

        protected override void LoadingCompled(StatePair<LoadingCompled, EmptyState> message)
        {
            SentUpdate();
            base.LoadingCompled(message);
        }

        private void SentUpdate()
        {
            GetGlobalComponent<TextLayerComponent>()
               .Select(textData =>
                       {
                           string title = string.Empty;
                           string main = string.Empty;
                           string detail = string.Empty;

                           foreach (var (titleData, mainDescription, detailData) in textData.TextData)
                           {
                               if (string.IsNullOrWhiteSpace(title) && !string.IsNullOrWhiteSpace(titleData))
                                   title = titleData;
                               if (string.IsNullOrWhiteSpace(main) && !string.IsNullOrWhiteSpace(mainDescription))
                                   main = mainDescription;
                               if (string.IsNullOrWhiteSpace(detail) && !string.IsNullOrWhiteSpace(detailData))
                                   detail = detailData;
                           }

                           return (title, main, detail);
                       })
               .ObserveOnSelf()
               .SubscribeWithStatus(r => EmitEvents(new TextEvent(r.title, r.main, r.detail)));
        }
    }
}