using Akka.Actor;
using System;
using System.Threading.Tasks;
using TextAdventures.Builder;
using TextAdventures.Engine;
using TextAdventures.Engine.Actors;

namespace Playground
{
    internal class Program 
    {
        private sealed class Killer : GameProcess
        {
            public Killer() => Receive<string>(Wait);

            private void Wait(string obj)
            {
                Console.WriteLine(obj);
                Console.ReadKey();
                Context.System.Terminate();
            }

            protected override void LoadingCompled(LoadingCompled obj)
            {
                Console.WriteLine();
                Self.Tell("Taste zum Beenden");

                base.LoadingCompled(obj);
            }
        }

        private static async Task Main()
        {
            World world = new();

            world.WithProcess<Killer>("Killer")
                 .WithErrorHandler(e => Console.WriteLine($"Fehler: {e}"))
                 .WithGameObject(new GameObjectBlueprint("Test1")
                                    .WithComponent<TestComponent>())
                 .WithGameObject(new GameObjectBlueprint("Test2")
                                    .WithComponent<TestComponent>());


            await Game.Create("TestGame", world).System.WhenTerminated;
        }
    }
}