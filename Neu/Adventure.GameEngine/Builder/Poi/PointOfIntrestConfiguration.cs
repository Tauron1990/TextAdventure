using Adventure.GameEngine.Builder.Core;
using Adventure.GameEngine.Builder.Events;
using Adventure.GameEngine.Core;
using Adventure.GameEngine.Core.Blueprints;
using Adventure.GameEngine.Systems.Components;
using JetBrains.Annotations;

namespace Adventure.GameEngine.Builder.Poi
{
    [PublicAPI]
    public sealed class PointOfIntrestConfiguration<TEventSource, TRoot> : IEventable<TEventSource, PointOfInterst>
        where TEventSource : IWithMetadata, IEntityConfiguration
        where TRoot : IHasRoot
    {
        private readonly TEventSource _eventSource;
        private readonly PointOfInterst _pointOfInterst = new PointOfInterst();

        public PointOfIntrestConfiguration(TEventSource eventSource, TRoot root)
        {
            _eventSource = eventSource;
            root.Root.AddBlueprint(new PointOfIntrestAddr(_pointOfInterst));

        }

        public PointOfIntrestConfiguration<TEventSource, TRoot> SetShowTo(bool show)
        {
            _pointOfInterst.Show = show;
            return this;
        }

        public PointOfIntrestConfiguration<TEventSource, TRoot> WithText(LazyString text)
        {
            _pointOfInterst.Text = text;
            return this;
        }

        TEventSource IEventable<TEventSource, PointOfInterst>.EventSource => _eventSource;

        PointOfInterst IEventable<TEventSource, PointOfInterst>.EventData => _pointOfInterst;
    }
}