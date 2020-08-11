using System;
using System.Windows;
using Adventure.GameEngine.Components;
using LazyData.Binary;
using LazyData.Mappings.Mappers;
using LazyData.Mappings.Types;
using LazyData.Registries;

namespace TextAdventure
{
    class Program
    {
        [STAThread]
        static int Main(string[] args)
        {
            return new Application().Run(new MainWindow());
        }
    }
}
