using System;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Akka.Actor;
using TextAdventure.GameFactory;
using TextAdventures.Builder;
using TextAdventures.Engine;

namespace TextAdventure
{
    /// <summary>
    ///     Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private static readonly Guid ProfileNameBase = new("c8e4ac50-0015-4677-83b3-e729d8716bd1");

        private GameMaster? _gameMaster;

        public MainWindow()
            => InitializeComponent();

        protected override void OnClosing(CancelEventArgs e)
        {
            _gameMaster?.Stop().Wait();
            base.OnClosing(e);
        }

        private void MainWindow_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            var savePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                                        "Tauron", "TextAdventure", "Saves");

            Saves.Init(savePath);
            Saves.NewGame += (profileName, saveName, newGame) =>
                             {
                                 try
                                 {
                                     string profileDic = Path.Combine(savePath, GuidFactories.Deterministic.Create(ProfileNameBase, profileName).ToString("D"));

                                     var world = World.Create(profileDic, profileName);

                                     world.Add(Props.Create(() => new MainWindowSubscriber()), "Main_Window_Subscriber");
                                     Saves.Prepare(world);
                                     MainContent.PrepareGame(world);

                                     GameWorld.ConfigurateWorld(world);

                                     _gameMaster = Game.Create(world, newGame)
                                                       .Start(saveName, Fail);
                                 }
                                 catch (Exception exception)
                                 {
                                     Fail(exception);
                                 }
                             };
        }

        private void Fail(Exception e)
        {
            var builder = new StringBuilder();
            builder.AppendLine("Fehler beim Starten:");
            builder.AppendLine();
            builder.AppendLine(e.ToString());

            MainContent.Display(builder.ToString());
            _gameMaster?.Stop();
        }

        private sealed class MainWindowSubscriber : DomainEventSubscriber { }
    }
}