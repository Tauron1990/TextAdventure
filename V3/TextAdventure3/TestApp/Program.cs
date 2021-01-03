using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.DI.Core;
using Autofac;
using FluentValidation;
using Serilog;
using Tauron;
using Tauron.Akka;
using Tauron.Application.Workshop.Mutating;
using Tauron.Application.Workshop.Mutating.Changes;
using Tauron.Application.Workshop.Mutation;
using Tauron.Application.Workshop.StateManagement;
using Tauron.Application.Workshop.StateManagement.Attributes;
using Tauron.Application.Workshop.StateManagement.DataFactorys;
using Tauron.Host;
using Tauron.Operations;

namespace TestApp
{
    internal static class Program
    {
        //#region Data

        //public sealed partial record UserData(string Id, string Name, DateTime CreationTime) : IStateEntity
        //{
        //    public DateTime LastModifed { get; } = DateTime.Now;

        //    public string Tag { get; init; }

        //    public bool Delete { get; init; }

        //    public bool IsNew { get; init; }
        //}

        //public sealed partial record UserData : ICanApplyChange<UserData>
        //{
        //    public UserData Apply(MutatingChange apply)
        //        => apply switch
        //        {
        //            NewUserEvent newUser => this with { Name = newUser.Name },
        //            _ => this
        //        };
        //}

        //#endregion

        //#region Querys

        //public sealed record NamedUserQuery(string Name) : IQuery
        //{
        //    public string ToHash() => Name;
        //}

        //public sealed record IdUserQuery(string Id) : IQuery
        //{
        //    public string ToHash() => Id;
        //}

        //#endregion

        //#region DataSource

        //[DataSource]
        //public sealed class UserDataSource : MapSourceFactory
        //{
        //    private sealed class ActualSource : IExtendedDataSource<UserData?>
        //    {
        //        private readonly ConcurrentDictionary<string, UserData> _datas;
        //        public ActualSource(ConcurrentDictionary<string, UserData> datas) => _datas = datas;
                
        //        public Task<UserData?> GetData(IQuery query)
        //        {
        //            return Task.Run(() =>
        //                            {
        //                                UserData? data;
                                        
        //                                switch (query)
        //                                {
        //                                    case IdUserQuery idUser:
        //                                        if (!_datas.TryGetValue(idUser.Id, out data))
        //                                            data = new UserData(idUser.Id, string.Empty, DateTime.Now) {IsNew = true};
        //                                        break;
        //                                    case NamedUserQuery namedUser:
        //                                        data = _datas.Values.SingleOrDefault(d => d.Name == namedUser.Name) 
        //                                             ?? new UserData(Guid.NewGuid().ToString(), namedUser.Name, DateTime.Now){IsNew = true};
        //                                        break;
        //                                    default:
        //                                        data = null;
        //                                        break;
        //                                }

        //                                return data;
        //                            });
        //        }

        //        public Task SetData(IQuery query, UserData? data)
        //        {
        //            if (data == null)
        //                return Task.CompletedTask;

        //            if (data.Delete && _datas.ContainsKey(data.Id))
        //                _datas.TryRemove(data.Id, out _);
        //            else
        //                _datas[data.Id] = data with{IsNew = false};

        //            return Task.CompletedTask;
        //        }

        //        public Task OnCompled(IQuery query) => Task.CompletedTask;
        //    }

        //    public UserDataSource()
        //    {
        //        var data = new ConcurrentDictionary<string, UserData>();
        //        Register<ActualSource, UserData?>(() => new ActualSource(data));
        //    }
        //}

        //#endregion

        //#region DataTransFormation

        //public sealed record NewUserCommand(string Name) : IStateAction
        //{
        //    public string ActionName => nameof(NewUserCommand);
        //    public IQuery Query { get; } = new NamedUserQuery(Name);
        //}

        //public sealed record NewUserEvent(string Name, string Id, DateTime CreationTime) : MutatingChange;
        
        //[BelogsToState(typeof(UserState))]
        //public static class UserMutation
        //{
        //    [Validator]
        //    public static IValidator<NewUserCommand> UserCommandValidator { get; } = new InlineValidator<NewUserCommand>()
        //                                                                             {
        //                                                                                 c => c.RuleFor(p => p.Name).NotEmpty()
        //                                                                             };

        //    [Reducer]
        //    public static ReducerResult<UserData> RunNewUserCommand(MutatingContext<UserData> context, NewUserCommand command) 
        //        => !context.Data.IsNew 
        //               ? ReducerResult.Fail(context, "User Existiert Schon") 
        //               : ReducerResult.Sucess(context.WithChange(new NewUserEvent(command.Name, context.Data.Id, context.Data.CreationTime)));
        //}
        
        //[State]
        //public sealed class UserState: StateBase<UserData>
        //{
        //    public UserState(ExtendedMutatingEngine<MutatingContext<UserData>> engine) : base(engine) { }
        //}

        //#endregion

        //public sealed class ConsoleActor : ExpandedReceiveActor
        //{
        //    public ConsoleActor(IActionInvoker invoker)
        //    {
        //        static (string Command, string[] Args) ParseCommand(string? input)
        //        {
        //            if (string.IsNullOrWhiteSpace(input))
        //                return (string.Empty, Array.Empty<string>());

        //            string[] split = input.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
                    
        //            return split.Length == 1 
        //                       ? (split[0], Array.Empty<string>()) 
        //                       : (split[0], split.Skip(1).ToArray());
        //        }

        //        static Func<IStateAction?> CommadAction((string Command, string[] args) commandTuple)
        //        {
        //            var (command, args) = commandTuple;

        //            return command switch
        //            {
        //                "new" => () =>
        //                         {
        //                             Console.Write("Neuer Benutzer: ");
        //                             return new NewUserCommand(args.FirstOrDefault() ?? string.Empty);
        //                         },
        //                "exit" => () =>
        //                          {
        //                              Console.WriteLine("Beende Anwendung");
        //                              Context.System.Terminate();
        //                              return null;
        //                          },
        //                _ => () =>
        //                     {
        //                         Console.WriteLine("Unkanntes kommando");
        //                         return null;
        //                     }
        //            };
        //        }

        //        Receive<AppRoute>(Start);
        //        Receive<IOperationResult>(ActionResult);
                
        //        WhenReceiveSafe<Starter>(start 
        //                                 => start
        //                                    .SelectMany(_ =>
        //                                                {
        //                                                    Console.WriteLine();
        //                                                    return Task.Run(Console.ReadLine);
        //                                                })
        //                                   .Select(ParseCommand)
        //                                   .Select(CommadAction)
        //                                   .ObserveOn(ActorScheduler.CurrentSelf)
        //                                   .Select(c => c())
        //                                   .ExecuteCommands(invoker));
        //    }

        //    private void ActionResult(IOperationResult obj)
        //    {
        //        if(obj.Ok)
        //            Console.WriteLine("Kommando Ausgeführt");
        //        else
        //        {
        //            switch (obj.Outcome)
        //            {
        //                case NewUserCommand:
        //                    Console.WriteLine("Benutzer Konnte nicht erstellt werden:");
        //                    break;
        //            }

        //            Console.WriteLine(obj.Error);
        //        }

        //        Self.Tell(Starter.Inst);
        //    }

        //    private void Start(AppRoute obj)
        //    {
        //        Console.WriteLine("--Programm Gestartet--");
        //        Self.Tell(Starter.Inst);
        //    }

        //    private sealed record Starter
        //    {
        //        public static readonly Starter Inst = new();
        //    }
        //}
        
        //private sealed class AppRoute : IAppRoute
        //{
        //    private readonly ActorSystem _system;

        //    public AppRoute(ActorSystem system) => _system = system;

        //    public Task ShutdownTask { get; } = Task.CompletedTask;
        //    public Task WaitForStartAsync(ActorSystem actorSystem)
        //    {
        //        _system.ActorOf(_system.DI().Props(typeof(ConsoleActor))).Tell(this);
        //        return Task.CompletedTask;
        //    }
        //}
        
        //private static async Task Main(string[] args)
        //{
        //    Console.Title = "Test App";
            
        //    using var system = ActorApplication.Create(args)
        //                                       .ConfigureAutoFac(b =>
        //                                                         {
        //                                                             b.RegisterType<AppRoute>().As<IAppRoute>();
        //                                                             b.RegisterStateManager(true, (builder, context) =>
        //                                                                                              builder
        //                                                                                                 .AddFromAssembly<UserData>(context)
        //                                                                                                 .WithDefaultSendback(true));
        //                                                         })
        //                                       .Build();

        //    await system.Run();
        //}

        public static void Main()
        {
            var sub = Observable.Return("Hallo Welt");
            var error = sub.Where(_ => throw new InvalidOperationException("Test Error"));

            error.Subscribe(Console.WriteLine, e => Console.WriteLine(e.Message));
            sub.Subscribe(Console.WriteLine, e => Console.WriteLine(e.Message));
            error.Subscribe(Console.WriteLine, e => Console.WriteLine(e.Message));
            sub.Subscribe(Console.WriteLine, e => Console.WriteLine(e.Message));
        }
    }
}