using System.IO;
using TextAdventures.Builder;
using TextAdventures.Engines;

namespace Playground
{
    class Program
    {
        static void Main(string[] args)
        {
            var save = Path.GetFullPath("test.dat");
            var game = Game.Create(World.Create(save), true);
            var master = game.Start();
            master.Stop().Wait();

            //var builder = new SqliteConnectionStringBuilder {DataSource = "data.db", Mode = SqliteOpenMode.ReadWriteCreate, Cache = SqliteCacheMode.Shared};
            //var builder1 = $"akka.persistence.journal.sqlite.connection-string : \"{builder.ConnectionString}\"\n" +
            //    $"akka.persistence.snapshot-store.sqlite.connection-string : \"{builder.ConnectionString}\"";

            //var one = ConfigurationFactory.ParseString(builder1);
            //var testConfig = one.WithFallback(ConfigurationFactory.ParseString(File.ReadAllText("akka.conf")));
            //var testString = testConfig.ToString(true);

            //var temp = one.GetString("akka.persistence.journal.sqlite.connection-string");

            //using var test = ActorSystem.Create("Test", testConfig);

            //var testActor = test.ActorOf<TestPersitence>();
            //testActor.Tell(new TestCommand());

            //TestPersitence.Waiter.WaitOne();
            //var test2 = PersistenceQuery.Get(test).ReadJournalFor<SqlReadJournal>(SqlReadJournal.Identifier)
            //   .EventsByPersistenceId(TestPersitence.TestID, 0, long.MaxValue);

            //using var mat = test.Materializer();
            //test2.RunForeach(ee =>
            //{
            //    Console.WriteLine(ee.Event);
            //    TestPersitence.Waiter.Set();
            //}, mat);

            //TestPersitence.Waiter.WaitOne();
        }
    }
}
