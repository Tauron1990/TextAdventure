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

        private IObservableGroup? _gameInfo;

        public MainContent() => InitializeComponent();

        public void LoadGame(Game game)
        {
            Dispatcher.Invoke(() =>
            {
                _gameInfo = game.ObservableGroupManager.GetObservableGroup(new Group(typeof(GameInfo)));

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

                var data = _gameInfo.SingleOrDefault()?.GetComponent<GameInfo>();
                if(data != null)
                    UpdateContent(new UpdateTextContent(data.LastContent, data.LastDescription), true);
            });
        }

        public void UnloadGame() => _disposable?.Dispose();

        private void UpdateComands(IEnumerable<string> data) 
            => CommandBox.ItemsSource = data.ToArray();

        private void UpdateContent(UpdateTextContent update, bool ignoreGameInfo = false)
        {
            if (update.Description != null)
            {
                _description.Inlines.Clear();
                _description.Inlines.Add(new Run(update.Description));
                UpdateGameInfo(gi => gi.LastDescription = update.Description, ignoreGameInfo);
            }

            if (update.Content != null)
            {
                _content.Inlines.Clear();
                _content.Inlines.Add(new Run(update.Content));
                UpdateGameInfo(gi => gi.LastContent = update.Content, ignoreGameInfo);
            }
        }

        private void UpdateGameInfo(Action<GameInfo> info, bool ignore)
        {
            if(ignore || _gameInfo == null) return;

            _gameInfo.Select(e => e.GetComponent<GameInfo>()).ForEachRun(info);
        }

        public void Display(string content) => Dispatcher.Invoke(() => UpdateContent(new UpdateTextContent(string.Empty, content), true));

        private void Input_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            
            _eventSystem.Publish(CommandBox.Text);
            CommandBox.Text = string.Empty;
        }
    }
}
