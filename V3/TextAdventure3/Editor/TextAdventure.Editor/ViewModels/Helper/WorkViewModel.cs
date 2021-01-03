using System;
using Autofac;
using Tauron;
using Tauron.Application.CommonUI.AppCore;
using Tauron.Application.CommonUI.Model;
using Tauron.Application.Workshop.StateManagement;
using TextAdventure.Editor.Data.ProjectData;
using TextAdventure.Editor.Operations;

namespace TextAdventure.Editor.ViewModels.Helper
{
    public abstract class WorkViewModel : StateUIActor
    {
        private static GameProject CreateDummy()
            => GameProject.Create("Kein Projekt", new Version(0, 0));

        protected WorkViewModel(ILifetimeScope lifetimeScope, IUIDispatcher dispatcher, IActionInvoker actionInvoker)
            : base(lifetimeScope, dispatcher, actionInvoker)
        {
            IsValid = RegisterProperty<bool>(nameof(IsValid)).WithDefaultValue(false);
            CurrentProject = RegisterProperty<GameProject>(nameof(CurrentProject)).WithDefaultValue(CreateDummy());

            ConfigurateState<IOState>(s =>
                                      {
                                          s.ProjectLoadFailed.Subscribe(_ =>
                                                                        {
                                                                            CurrentProject += CreateDummy();
                                                                            IsValid += false;
                                                                        }).DisposeWith(this);
                                          s.ProjectLoaded.Subscribe(d =>
                                                                    {
                                                                        CurrentProject += d.Project;
                                                                        IsValid += true;
                                                                    }).DisposeWith(this);
                                      });
        }

        public UIProperty<GameProject> CurrentProject { get; set; }

        public UIProperty<bool> IsValid { get; set; }
    }
}