using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using TextAdventures.Engine.Actors;
using TextAdventures.Engine.Systems;

namespace TextAdventures.Engine.Modules.Command
{
    public sealed class CommandCoordinator : CoordinatorProcess, IConsumeEvent<UpdateCommandLayerEvent>
    {
        private readonly Task<CommandLayerComponent> _textData;
        private CommandLayerComponent CommandData => _textData.Result;

        public CommandCoordinator() 
            => _textData = GetGlobalComponent<CommandLayerComponent>();

        public void Process(UpdateCommandLayerEvent evt)
        {

            EmitEvents(CommandData, evt.Data.Select(d => new CommandLayerEvent(d.Key, d.Value)).Cast<object>().ToArray());
            SentUpdate();
        }

        protected override void LoadingCompled(LoadingCompled obj) 
            => SentUpdate();

        private void SentUpdate()
        {
            var data = CommandData.CommandData.GroupBy(d => d.Category)
                                  .ToImmutableDictionary(g => g.Key, g => ImmutableList<Type>.Empty.AddRange(g.Select(d => d.TargetType)));

            EmitEvents(new CommandEvent(data));
        }
    }
}