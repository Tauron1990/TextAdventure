using System;
using System.IO;
using System.Reactive.Linq;
using Autofac;
using JetBrains.Annotations;
using MaterialDesignExtensions.Controls;
using MaterialDesignThemes.Wpf;
using Tauron;
using Tauron.Akka;
using Tauron.Application.CommonUI;
using Tauron.Application.CommonUI.AppCore;
using Tauron.Application.CommonUI.Model;
using Tauron.Application.Workshop.StateManagement;
using TextAdventure.Editor.Operations;
using TextAdventure.Editor.Operations.Command;
using TextAdventure.Editor.ViewModels.Messages;
using TextAdventure.Editor.Views.Dialogs;

namespace TextAdventure.Editor.ViewModels
{
    [UsedImplicitly]
    public sealed class MainWindowViewModel : StateUIActor
    {
        public MainWindowViewModel(ILifetimeScope lifetimeScope, IUIDispatcher dispatcher, IActionInvoker actionInvoker)
            : base(lifetimeScope, dispatcher, actionInvoker)
        {
            Messages = RegisterProperty<ISnackbarMessageQueue>(nameof(Messages)).WithDefaultValue(new SnackbarMessageQueue(TimeSpan.FromSeconds(5)));
            WindowTitle = RegisterProperty<string>(nameof(WindowTitle)).WithDefaultValue("Text Adventure Editor");
            Dashboard = this.RegisterViewModel<DashboardViewModel>(nameof(Dashboard));

            NewProject = NewCommad
                        .ThenFlow(o => o.Dialog(this).Of<NewProjectDialog, TryLoadProjectCommand?>()
                                        .ToActionInvoker(ActionInvoker))
                        .ThenRegister("NewProject");


            OpenProject = NewCommad
                         .ThenFlow(o => o.Select(_ => new OpenDirectoryDialogArguments {CurrentDirectory = NewProjectDialogModel.ProjectPath()})
                                         .Dialog(this).Of<GenericOpenDictionaryDialog, string?>()
                                         .NotNull().Where(Directory.Exists)
                                         .Select(s => new TryLoadProjectCommand(s, false, string.Empty))
                                         .ToActionInvoker(actionInvoker))
                         .ThenRegister("OpenProject");

            ConfigurateState<IOState>(s =>
                                      {
                                          s.ProjectLoadFailed.Subscribe(lf =>
                                                                        {
                                                                            WindowTitle += "Text Adventure Editor";
                                                                            Messages.Value.Enqueue($"Laden Fhelgeschlagen: {lf.Message}");
                                                                        }).DisposeWith(this);

                                          s.ProjectLoaded.Subscribe(pl =>
                                                                    {
                                                                        var (project, source) = pl;

                                                                        WindowTitle += $"Text Adventure Editor ({project.GameName} -- {project.GameVersion})";
                                                                        Messages.Value.Enqueue($"Laden Erfolgreich: {source}");
                                                                    }).DisposeWith(this);

                                          s.ProjectSaved.Subscribe(ps => Messages.Value.Enqueue(ps.IsOk ? "Projekt geschpeichert" : $"Speichern Fehlgeschlagen: {ps.Error}")).DisposeWith(this);


                                      });

            this.SubscribeToEvent<MainWindowMessage>(msg => Messages.Value.Enqueue(msg.Message));
        }

        public UIModel<DashboardViewModel> Dashboard { get; }

        public UIProperty<string> WindowTitle { get; set; }

        public UIPropertyBase? NewProject { get; }

        public UIPropertyBase? OpenProject { get; }

        public UIProperty<ISnackbarMessageQueue> Messages { get; }
    }
}