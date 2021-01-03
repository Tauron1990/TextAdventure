using System;
using System.IO;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using DynamicData.Binding;
using ReactiveUI;
using TextAdventure.Editor.Models;
using TextAdventure.Editor.ViewModels.Shared;
using TextAdventure.Editor.Views;

namespace TextAdventure.Editor.ViewModels
{
    public sealed class NewProjectViewModel : ViewModelBase
    {
        private string _projectName = string.Empty;
        private string _gameName = string.Empty;
        private string _projectPath = string.Empty;
        private bool _initGit = true;
        private bool _isRunning;

        public string ProjectName
        {
            get => _projectName;
            set => this.RaiseAndSetIfChanged(ref _projectName, value);
        }

        public string GameName
        {
            get => _gameName;
            set => this.RaiseAndSetIfChanged(ref _gameName, value);
        }

        public string ProjectPath
        {
            get => _projectPath;
            set => this.RaiseAndSetIfChanged(ref _projectPath, value);
        }

        public bool InitGit
        {
            get => _initGit;
            set => this.RaiseAndSetIfChanged(ref _initGit, value);
        }

        public bool IsRunning
        {
            get => _isRunning;
            set
            {
                App.Dispatcher.InvokeAsync(() =>
                                           {
                                               this.RaiseAndSetIfChanged(ref _isRunning, value);
                                               this.RaisePropertyChanged(nameof(IsNotRunning));
                                           });
            }
        }

        public bool IsNotRunning => !_isRunning;
        
        public ICommand CancelCommand { get; }

        public ICommand NextCommand { get; }
        
        public NewProjectViewModel()
        {
            // ReSharper disable once RedundantAssignment
            char[] invalidChars = Path.GetInvalidPathChars();

            bool IsValidPath(string path)
            {
                return path.All(c => !invalidChars.Contains(c));
            }

            CancelCommand = ReactiveCommand.Create(Workspace.Shared.Reset, this.WhenChanged(m => m.IsNotRunning, (_, r) => r));

            var canNext = from pm in this.WhenChanged(m => m.ProjectName, (_, s) => s)
                          from gm in this.WhenChanged(m => m.GameName, (_, s) => s)
                          from pp in this.WhenChanged(m => m.ProjectPath, (_, s) => s)
                          from run in this.WhenChanged(m => m.IsRunning, (_, r) => r)
                          select !string.IsNullOrWhiteSpace(pm) && !string.IsNullOrWhiteSpace(gm)
                              && !string.IsNullOrWhiteSpace(pp) && IsValidPath(pp) && !run;
            
            var next = ReactiveCommand.Create(() => new SetupProject(ProjectName, GameName, ProjectPath, InitGit), canNext, TaskPoolScheduler.Default);

            NextCommand = next;

            this.WhenActivated(cd =>
                               {
                                  
                                   this.WhenChanged(m => m.ProjectName, (_, s) => s).Throttle(TimeSpan.FromMilliseconds(100))
                                       .ObserveOn(RxApp.MainThreadScheduler)
                                       .Subscribe(s => GameName = s)
                                       .DisposeWith(cd);

                                   this.WhenChanged(m => m.GameName, (_, s) => s).Throttle(TimeSpan.FromMilliseconds(100))
                                       .ObserveOn(RxApp.MainThreadScheduler)
                                       .Subscribe(s => ProjectPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "TextAdventureProjects", s))
                                       .DisposeWith(cd);
                                   
                                   next.Subscribe(s =>
                                                  {
                                                      IsRunning = true;
                                                      Workspace.Shared.Setup(s, AskDelete)
                                                               .Where(r => !string.IsNullOrWhiteSpace(r))
                                                               .Subscribe(r =>
                                                                          {
                                                                              MessageBus.SendMessage(new PropagateError(r!, true));
                                                                              IsRunning = false;
                                                                          },
                                                                          e =>
                                                                          {
                                                                              MessageBus.SendMessage(new PropagateError(e.ToString(), true));
                                                                              IsRunning = false;
                                                                          },
                                                                          Workspace.Shared.Reset);
                                                  })
                                       .DisposeWith(cd);
                               });

        }

        private IObservable<bool> AskDelete(IObservable<string> arg)
        {
            static Task<bool> AskUser(string path)
                => App.Dispatcher.InvokeAsync(async () =>
                                              {
                                                  var box = new QuestionBox("Projekt Übserschreiben", $"{path} \n Der Ortner wird wärend des Setups Gelöscht. Fortfahren?");
                                                  Thread.Sleep(100);
                                                  return await box.ShowDialog<bool>(App.MainWindow);
                                              });

            return from path in arg
                   from result in Observable.StartAsync(() => AskUser(path))
                   select result;
        }
    }
}