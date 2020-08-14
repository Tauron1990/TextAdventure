using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Windows.Documents;
using System.Windows.Input;
using Adventure.GameEngine;
using Adventure.GameEngine.Components;
using Adventure.GameEngine.Events;
using EcsRx.Events;
using EcsRx.Extensions;
using EcsRx.Groups;
using EcsRx.Groups.Observable;
using EcsRx.Infrastructure.Events;
using EcsRx.MicroRx.Events;

namespace Adventure.Ui
{
    /// <summary>
    /// Interaktionslogik für MainContent.xaml
    /// </summary>
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

        public void UnloadGame() => _disposable?.Dispose();

        private void UpdateComands(IEnumerable<string> data) 
            => CommandBox.ItemsSource = data.ToArray();

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

        private void Input_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            
            _eventSystem.Publish(CommandBox.Text);
            CommandBox.Text = string.Empty;
        }
    }
}
