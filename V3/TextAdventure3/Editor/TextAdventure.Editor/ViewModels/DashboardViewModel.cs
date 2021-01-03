using System;
using System.Reactive.Linq;
using Autofac;
using JetBrains.Annotations;
using Tauron;
using Tauron.Application.CommonUI.AppCore;
using Tauron.Application.CommonUI.Model;
using Tauron.Application.Workshop.StateManagement;
using Tauron.Operations;
using TextAdventure.Editor.ViewModels.Helper;

namespace TextAdventure.Editor.ViewModels
{
    [UsedImplicitly]
    public sealed class DashboardViewModel : WorkViewModel
    {
        public DashboardViewModel(ILifetimeScope lifetimeScope, IUIDispatcher dispatcher, IActionInvoker actionInvoker)
            : base(lifetimeScope, dispatcher, actionInvoker)
        {
            IsCommonDataInEdit = RegisterProperty<bool>(nameof(IsCommonDataInEdit)).WithDefaultValue(false);

            GameName = RegisterProperty<string>(nameof(GameName))
                      .WithDefaultValue(string.Empty)
                      .WithValidator(o => o.Select(s => string.IsNullOrWhiteSpace(s) ? new Error("GameName darf nicht Leer Sein", "Name") : null));

            GameVersion = RegisterProperty<string>(nameof(GameVersion))
                         .WithDefaultValue(string.Empty)
                         .WithValidator(o => o.Select(s => Version.TryParse(s, out _) ? null : new Error("GameName darf nicht Leer Sein", "Version")));

            CurrentProject.Subscribe(d =>
                                     {
                                         GameName += d.GameName;
                                         GameVersion += d.GameVersion.ToString();

                                     }).DisposeWith(this);

            EditCommonCommand = NewCommad
                               .WithExecute(() => IsCommonDataInEdit += true)
                               .ThenRegister(nameof(EditCommonCommand));

            EditCommonBackCommand = NewCommad
                                   .WithExecute(() =>
                                                {
                                                    GameName += CurrentProject.Value.GameName;
                                                    GameVersion += CurrentProject.Value.GameVersion.ToString();
                                                    IsCommonDataInEdit += false;
                                                })
                                   .ThenRegister(nameof(EditCommonBackCommand));
        }
        
        public UIPropertyBase? ApplyCommonEditCommand { get; }

        public UIPropertyBase? EditCommonBackCommand { get; }

        public UIPropertyBase? EditCommonCommand { get; }

        public UIProperty<bool> IsCommonDataInEdit { get; set; }

        public UIProperty<string> GameName { get; set; }

        public UIProperty<string> GameVersion { get; set; }
    }
}