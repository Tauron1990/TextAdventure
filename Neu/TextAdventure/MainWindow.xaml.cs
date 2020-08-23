using System;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Adventure.GameEngine;
using Adventure.GameEngine.ContentManagment;

namespace TextAdventure
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : IStartUpNotify
    {
        private AdventureGame? _game;

        private readonly ContentManagement _management = new ContentManagement();

        public MainWindow() => InitializeComponent();

        protected override void OnClosing(CancelEventArgs e)
        {
            _game?.StopApplication();
            base.OnClosing(e);
        }

        private void MainWindow_OnKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Escape)
                Close();
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            var savePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Tauron",
                "TextAdventure",
                "Saves");

            Saves.Init(savePath);
            Saves.LoadGameEvent += s =>
            {
                MainContent.UnloadGame();
                _game?.StopApplication();

                _game = new AdventureGame(s, this, _management);

                Task.Run(() =>
                {
                    _game.StartApplication();
                });
            };
        }

        public void Fail(Exception e)
        {
            var builder = new StringBuilder();
            builder.AppendLine("Fehler beim Starten:");
            builder.AppendLine();
            builder.AppendLine(e.ToString());

            MainContent.Display(builder.ToString());
            _game?.StopApplication();
        }

        public void Succed(Game game) 
            => MainContent.LoadGame(game);
    }
}
