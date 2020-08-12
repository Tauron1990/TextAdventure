using System;
using System.Windows;
using Adventure.GameEngine.Components;
using Adventure.GameEngine.Internal;
using Newtonsoft.Json;

namespace TextAdventure
{
    class Program
    {
        [STAThread]
        static int Main(string[] args)
        {
            var testColl = new EntityCollectionData(1);
            var testEnt = new EntityData();

            testEnt.Components.Add(new ReplayInfo());
            testEnt.Components.Add(new GameInfo(1, DateTimeOffset.Now));

            testColl.Entitys.Add(testEnt);

            var result = JsonConvert.SerializeObject(testColl, Formatting.Indented);
            testColl = JsonConvert.DeserializeObject<EntityCollectionData>(result);

            return new Application().Run(new MainWindow());
        }
    }
}
