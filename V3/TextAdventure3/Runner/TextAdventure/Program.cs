using System;
using System.Windows;

namespace TextAdventure
{
    internal class Program
    {
        [STAThread]
        private static int Main(string[] args) => new Application().Run(new MainWindow());
    }
}