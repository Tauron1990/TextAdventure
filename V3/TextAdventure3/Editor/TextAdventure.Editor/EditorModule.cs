using Autofac;
using Tauron.Application.CommonUI;
using TextAdventure.Editor.ViewModels;
using TextAdventure.Editor.Views;
using TextAdventure.Editor.Views.Dialogs;

namespace TextAdventure.Editor
{
    public sealed class EditorModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            //Views
            builder.RegisterView<MainWindow, MainWindowViewModel>();
            builder.RegisterView<DashboardView, DashboardViewModel>();

            //Dialogs
            builder.RegisterType<NewProjectDialog>().AsSelf();
            builder.RegisterType<GenericOpenDictionaryDialog>().AsSelf();

            base.Load(builder);
        }
    }
}