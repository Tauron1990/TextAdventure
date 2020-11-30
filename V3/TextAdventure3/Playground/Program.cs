using System;
using System.Threading.Tasks;
using Akka.Actor;
using TextAdventures.Builder;
using TextAdventures.Engine;
using TextAdventures.Engine.Actors;
using TextAdventures.Engine.CommandSystem;

namespace Playground
{
    internal class Program
    {
        private static async Task Main()
        {
            World world = new();

            world.WithProcess<Killer>("Killer")
                 .WithErrorHandler(e => Console.WriteLine($"Fehler: {e}"))
                 .WithCommandProcessor(CommandProcessor.RegistrationFor<TestCompenentCommandProcessor>());

            world.WithGameObject(new GameObjectBlueprint("Test1")
                                .WithComponent<TestComponent>()
                                .AddDefaultValue(nameof(TestComponent.Message), "Hallo From Test 1"));

            world.WithGameObject(new GameObjectBlueprint("Test2")
                                .WithComponent<TestComponent>()
                                .AddDefaultValue(nameof(TestComponent.Message), "Hello From Test 2"));


            var game =  Game.Create(new GameConfiguration("TestGame1", "TestProfile", "1"), world);

            await game.System.WhenTerminated;
        }

        private sealed class TestCompenentCommandProcessor : CommandProcessor<TestComponetCommand, TestComponent>
        {
            protected override void RunCommand(ICommandContext<TestComponent> commandContext, TestComponetCommand command) 
                => Console.WriteLine(commandContext.Component.Message);
        }

        private sealed class TestComponetCommand : IGameCommand
        {
            public string Target { get; }

            public TestComponetCommand(string target) => Target = target;
        }

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

                Game.GameMaster.SendCommand(new TestComponetCommand("Test1"));
                Game.GameMaster.SendCommand(new TestComponetCommand("Test2"));

                base.LoadingCompled(obj);
            }
        }
    }
}