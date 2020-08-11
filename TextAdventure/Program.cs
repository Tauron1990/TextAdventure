using System;
using System.Collections.Immutable;
using System.Threading;
using Adventure.GameEngine;
using EcsRx.Extensions;
using EcsRx.Groups;
using EcsRx.Plugins.Batching;
using EcsRx.Plugins.Computeds;
using EcsRx.Plugins.ReactiveSystems;
using EcsRx.Plugins.Views;
using Ninject;

namespace TextAdventure
{
    public sealed class TestGame : Game
    {
        public TestGame(IKernel kernel) 
            : base(kernel)
        {
        }

        protected override void ApplicationStarted()
        {
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var app = new TestGame(new StandardKernel());
            app.StartApplication();
        }
    }
}
