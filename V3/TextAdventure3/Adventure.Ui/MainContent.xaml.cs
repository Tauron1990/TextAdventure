using System;
using System.Linq;
using System.Windows.Documents;
using Akka.Actor;
using Akkatecture.Aggregates;
using Akkatecture.Subscribers;
using JetBrains.Annotations;
using TextAdventures.Builder;
using TextAdventures.Builder.Data.Command;
using TextAdventures.Engine;
using TextAdventures.Engine.Events;
using TextAdventures.Engine.Internal.Data.Aggregates;

namespace Adventure.Ui
{
    /// <summary>
    ///     Interaktionslogik für MainContent.xaml
    /// </summary>
    [PublicAPI]
    public partial class MainContent
    {
        private Paragraph   _content     = new();
        private Paragraph   _description = new();
        private GameMaster? _gameMaster;

        public MainContent()
        {
            InitializeComponent();
            CommandBox.CommandSelect += command => _gameMaster?.SendCommand(command);
        }

        public void PrepareGame(World game)
        {
            game.Add(Props.Create(() => new MainContentSubscriber(GameLoaded, UpdateCommands, UpdateContent)), "Main_Content_Subscriber");

            Dispatcher.Invoke(() =>
                              {
                                  var document = new FlowDocument();

                                  _description = new Paragraph();
                                  _content     = new Paragraph();

                                  document.Blocks.Add(_description);
                                  document.Blocks.Add(_content);

                                  TextContent.Document = document;
                              });
        }

        private void GameLoaded(GameLoaded loaded)
        {
            _gameMaster = loaded.Master;
            loaded.Master
                  .WhenTerminated
                  .ContinueWith(_ => CommandBox.UnloadGame());
        }

        private void UpdateCommands(CommandsUpdatedEvent update)
            => CommandBox.Update(update.Commands.Select(c => (c as ICommandMetadata, c)));

        private void UpdateContent(UpdateTextContent update)
        {
            if (update.Description != null)
            {
                _description.Inlines.Clear();
                _description.Inlines.Add(new Run(update.Description));
            }

            if (update.Content != null)
            {
                _content.Inlines.Clear();
                _content.Inlines.Add(new Run(update.Content));
            }
        }

        public void Display(string content) => Dispatcher.Invoke(() => UpdateContent(new UpdateTextContent(string.Empty, content)));

        private sealed class MainContentSubscriber : DomainEventSubscriber, ISubscribeTo<GameInfo, GameInfoId, CommandsUpdatedEvent>,
                                                     ISubscribeTo<GameInfo, GameInfoId, GameLoaded>, ISubscribeTo<GameInfo, GameInfoId, UpdateTextContent>
        {
            private readonly Action<GameLoaded>           _loaded;
            private readonly Action<CommandsUpdatedEvent> _updateCommand;
            private readonly Action<UpdateTextContent>    _updateText;

            public MainContentSubscriber(Action<GameLoaded> loaded, Action<CommandsUpdatedEvent> updateCommand, Action<UpdateTextContent> updateText)
            {
                _loaded        = loaded;
                _updateCommand = updateCommand;
                _updateText    = updateText;
            }

            public bool Handle(IDomainEvent<GameInfo, GameInfoId, CommandsUpdatedEvent> domainEvent)
            {
                _updateCommand(domainEvent.AggregateEvent);
                return true;
            }

            public bool Handle(IDomainEvent<GameInfo, GameInfoId, GameLoaded> domainEvent)
            {
                _loaded(domainEvent.AggregateEvent);
                return true;
            }

            public bool Handle(IDomainEvent<GameInfo, GameInfoId, UpdateTextContent> domainEvent)
            {
                _updateText(domainEvent.AggregateEvent);
                return true;
            }
        }
    }
}