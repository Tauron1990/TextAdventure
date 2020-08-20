using System;
using System.Windows;

namespace TextAdventure
{
    class Program
    {
        [STAThread]
        static int Main(string[] args) => new Application().Run(new MainWindow());
    }
}
