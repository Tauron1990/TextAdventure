using System;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace TextAdventure
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
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
        }


    }
}
