using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Windows.Documents;
using Adventure.GameEngine;
using Adventure.GameEngine.Commands;
using Adventure.GameEngine.Core;
using Adventure.GameEngine.Systems.Events;
using EcsRx.Events;
using EcsRx.Infrastructure.Events;
using EcsRx.MicroRx.Events;
using JetBrains.Annotations;

namespace Adventure.Ui
{
    /// <summary>
    /// Interaktionslogik für MainContent.xaml
    /// </summary>
    [PublicAPI]
    public partial class MainContent
    {
        private CompositeDisposable? _disposable;
        private IEventSystem _eventSystem = new EventSystem(new MessageBroker());

        private Paragraph _description = new Paragraph();
        private Paragraph _content = new Paragraph();

        public MainContent() => InitializeComponent();

        public void LoadGame(Game game)
        {
            Dispatcher.Invoke(() =>
            {
                CommandBox.InitGame(game);
                _disposable = new CompositeDisposable
                              {
                                  game.EventSystem.Receive<UpdateTextContent>().Subscribe(u => Dispatcher.Invoke(() => UpdateContent(u))),
                                  game.EventSystem.Receive<UpdateCommandList>().Subscribe(c => Dispatcher.Invoke(() => UpdateComands(c.Commands)))
                              };

                _eventSystem = game.EventSystem;

                var document = new FlowDocument();

                _description = new Paragraph();
                _content = new Paragraph();

                document.Blocks.Add(_description);
                document.Blocks.Add(_content);

                TextContent.Document = document;

                game.EventSystem.Publish(new QueryLastEntry());
            });
        }

        public void UnloadGame()
        {
            CommandBox.UnloadGame();
            _disposable?.Dispose();
        }

        private void UpdateComands(IEnumerable<(LazyString Name, Command command)> data)
            => CommandBox.Update(data);

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
    }
}
