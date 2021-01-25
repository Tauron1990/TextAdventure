using System;
using System.Reactive.Linq;
using Autofac;
using JetBrains.Annotations;
using Tauron;
using Tauron.Akka;
using Tauron.Application.CommonUI;
using Tauron.Application.CommonUI.AppCore;
using Tauron.Application.CommonUI.Model;
using Tauron.Application.Workshop.StateManagement;
using Tauron.Operations;
using TextAdventure.Editor.Operations;
using TextAdventure.Editor.Operations.Command;
using TextAdventure.Editor.ViewModels.Helper;
using TextAdventure.Editor.ViewModels.Messages;

namespace TextAdventure.Editor.ViewModels
{
    [UsedImplicitly]
    public sealed class DashboardViewModel : WorkViewModel
    {
        public DashboardViewModel(ILifetimeScope lifetimeScope, IUIDispatcher dispatcher, IActionInvoker actionInvoker)
            : base(lifetimeScope, dispatcher, actionInvoker)
        {
            #region Common Card

            CommonCardData = new CommonCard
                (
                 isCommonDataInEdit: RegisterProperty<bool>(nameof(CommonCard.IsCommonDataInEdit)).WithDefaultValue(false),

                 gameName: RegisterProperty<string>(nameof(CommonCard.GameName))
                          .WithDefaultValue(string.Empty)
                          .WithValidator(o => o.Select(s => string.IsNullOrWhiteSpace(s) ? new Error("Name darf nicht Leer Sein", "Name") : null)),

                 gameVersion: RegisterProperty<string>(nameof(CommonCard.GameVersion))
                             .WithDefaultValue(string.Empty)
                             .WithValidator(o => o.Select(s =>
                                                          {
                                                              if (string.IsNullOrWhiteSpace(s))
                                                                  return new Error("Version darf nicht Leer Sein", "Version");

                                                              return Version.TryParse(s, out _) ? null : new Error("Version Format: 0.0.0.0 oder 0.0", "Version");
                                                          })),

                 editCommonCommand: NewCommad
                                   .WithCanExecute(IsValid)
                                   .WithExecute(() => CommonCardData!.IsCommonDataInEdit += true)
                                   .ThenRegister(nameof(CommonCard.EditCommonCommand)),

                 editCommonBackCommand: NewCommad
                                       .WithExecute(() =>
                                                    {
                                                        CommonCardData!.GameName += CurrentProject.Value.GameName;
                                                        CommonCardData.GameVersion += CurrentProject.Value.GameVersion.ToString();
                                                        CommonCardData.IsCommonDataInEdit += false;
                                                    }).ThenRegister(nameof(CommonCard.EditCommonBackCommand)),

                 applyCommonEditCommand: NewCommad
                                        .WithCanExecute(from nameValid in Property(() => CommonCardData!.GameName).IsValid
                                                        from versionValid in Property(() => CommonCardData!.GameVersion).IsValid
                                                        select nameValid && versionValid)
                                        .ThenFlow(o => o.Select(_ => new ChangeGameNameCommand(CommonCardData!.GameName.Value, CommonCardData.GameVersion.Value))
                                                        .ToActionInvoker(ActionInvoker))
                                        .ThenRegister(nameof(CommonCard.ApplyCommonEditCommand)),

                 newCommand: NewCommad
                            .WithExecute(() => Context.System.EventStream.Publish(MainWindowCommand.New()))
                            .ThenRegister(nameof(CommonCard.NewCommand)),

                 openCommand: NewCommad
                             .WithExecute(() => Context.System.EventStream.Publish(MainWindowCommand.Open()))
                             .ThenRegister(nameof(CommonCard.OpenCommand))
                );

            #endregion

            WhenStateChanges<CommonDataState>()
               .FromEvent(cd => cd.NameChanged)
               .ToObservable(o =>
                             {
                                 o.Subscribe(_ => CommonCardData.IsCommonDataInEdit += false).DisposeWith(this);

                                 o.Where(c => c.HasChanged)
                                  .Subscribe(c =>
                                             {
                                                 var (_, newName, newVersion) = c;
                                                 CommonCardData.GameName += newName;
                                                 CommonCardData.GameVersion += newVersion.ToString();
                                             })
                                  .DisposeWith(this);

                                 o.Where(c => !c.HasChanged)
                                  .Subscribe(c =>
                                             {
                                                 var (_, newName, _) = c;
                                                 CommonCardData.GameName += newName;
                                                 CommonCardData.GameVersion += CurrentProject.Value.GameVersion.ToString();
                                             })
                                  .DisposeWith(this);
                             });

            CurrentProject.Subscribe(d =>
                                     {
                                         CommonCardData.GameName += d.GameName;
                                         CommonCardData.GameVersion += d.GameVersion.ToString();

                                     }).DisposeWith(this);
        }
        
        private CommonCard CommonCardData { get; }

        private EntitiesCard EntitiesCardData { get; }

        private sealed class EntitiesCard
        {
            public EntitiesCard(UIPropertyBase? editCommand) => EditCommand = editCommand;
            public UIPropertyBase? EditCommand { get; }
        }

        private sealed class CommonCard
        {
            public UIPropertyBase? OpenCommand { get; }

            public UIPropertyBase? NewCommand { get; }

            public UIPropertyBase? ApplyCommonEditCommand { get; }

            public UIPropertyBase? EditCommonBackCommand { get; }

            public UIPropertyBase? EditCommonCommand { get; }

            public UIProperty<bool> IsCommonDataInEdit { get; set; }

            public UIProperty<string> GameName { get; set; }

            public UIProperty<string> GameVersion { get; set; }

            public CommonCard(UIPropertyBase? applyCommonEditCommand, UIPropertyBase? editCommonBackCommand, UIPropertyBase? editCommonCommand, UIProperty<bool> isCommonDataInEdit, UIProperty<string> gameName, UIProperty<string> gameVersion, UIPropertyBase? openCommand, UIPropertyBase? newCommand)
            {
                ApplyCommonEditCommand = applyCommonEditCommand;
                EditCommonBackCommand = editCommonBackCommand;
                EditCommonCommand = editCommonCommand;
                IsCommonDataInEdit = isCommonDataInEdit;
                GameName = gameName;
                GameVersion = gameVersion;
                OpenCommand = openCommand;
                NewCommand = newCommand;
            }
        }
    }
}