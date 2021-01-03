using System;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Input;
using Avalonia.Controls;
using ReactiveUI;
using TextAdventure.Editor.Models;
using TextAdventure.Editor.Models.Data;
using TextAdventure.Editor.ViewModels.Shared;

namespace TextAdventure.Editor.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly ErrorDisplayViewModel _errorDisplay;
        private readonly MainContentViewModel _mainContentViewModel;

        private static Workspace Workspace => Workspace.Shared;
        
        private ViewModelBase _mainContent;
        private string _statusInfo = string.Empty;
        private string _title = "Text Adventure Editor";

        public ViewModelBase MainContent
        {
            get => _mainContent;
            set => this.RaiseAndSetIfChanged(ref _mainContent, value);
        }

        public string StatusInfo
        {
            get => _statusInfo;
            set => this.RaiseAndSetIfChanged(ref _statusInfo, value);
        }

        public string Title
        {
            get => _title;
            set => this.RaiseAndSetIfChanged(ref _title, value);
        }

        public ICommand OpenCommand { get; }
        
        public ICommand CloseProjectCommand { get; }
        
        public ICommand NewProjectCommand { get; }
        
        public MainWindowViewModel(ErrorDisplayViewModel errorDisplay, MainContentViewModel mainContent, Func<NewProjectViewModel> newProject)
        {
            _errorDisplay = errorDisplay;
            _mainContentViewModel = mainContent;
            _mainContent = mainContent;
            
            MainContent = mainContent;
            
            InitStatusAndErrorDisplay();
            InitProjectLoad();
            
            OpenCommand = InitOpen();
            CloseProjectCommand = ReactiveCommand.Create(Workspace.Reset);
            NewProjectCommand = ReactiveCommand.Create(() => MainContent = newProject());
            
            StatusInfo = "Bereit";
        }

        private void InitStatusAndErrorDisplay()
        {
            this.WhenActivated(cd =>
                               {
                                   Workspace.IsEmpty.Where(e => e).Subscribe(_ => MessageBus.SendMessage(new StatusBarInfo("Leeres Projekt"))).DisposeWith(cd);
                                   Workspace.ProjectInfo.Subscribe(pi => MessageBus.SendMessage(new StatusBarInfo($"Project wurde Geladen: {pi.ProjectName}"))).DisposeWith(cd);
                                   
                                   MessageBus.Listen<PropagateError>()
                                              .Subscribe(e =>
                                                         {
                                                             var (error, isFatal) = e;
                                                             if (isFatal)
                                                                 MainContent = _errorDisplay;
                                                             StatusInfo = error;
                                                         })
                                              .DisposeWith(cd);

                                   MessageBus.Listen<StatusBarInfo>()
                                             .Subscribe(i => StatusInfo = i.Info)
                                             .DisposeWith(cd);
                               });
        }

        private void InitProjectLoad()
        {
            this.WhenActivated(cd =>
                               {
                                   Workspace.ProjectReset
                                            .Subscribe(pi =>
                                                       {
                                                           if (string.IsNullOrWhiteSpace(pi.ProjectName))
                                                               Title = "Text Adventure Editor -" + "Kein Projekt";
                                                           else
                                                               Title = $"Text Adventure Editor: {pi.ProjectName}";
                                                           MainContent = _mainContentViewModel;
                                                       })
                                            .DisposeWith(cd);
                               });
        }
        
        private ICommand InitOpen()
        {
            static IObservable<string> OpenProjectDic()
            {
                return Observable.StartAsync(() =>
                                        {
                                            var diag = new OpenFolderDialog
                                                       {
                                                           Title = "Project Öffnen", 
                                                           Directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                                                       };

                                            return diag.ShowAsync(App.MainWindow);
                                        });
            }
            
            var command = ReactiveCommand.CreateFromObservable(OpenProjectDic, outputScheduler:TaskPoolScheduler.Default);

            this.WhenActivated(cd =>
                               {
                                   Workspace.Load(ProjectValidation.Validate(command))
                                            .Where(s => !string.IsNullOrWhiteSpace(s))
                                            .Subscribe(s => MessageBus.SendMessage(new PropagateError(s!, true)))
                                            .DisposeWith(cd);
                               });

            return command;
        }
    }
}
