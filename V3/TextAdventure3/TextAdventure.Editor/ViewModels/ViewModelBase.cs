using ReactiveUI;

namespace TextAdventure.Editor.ViewModels
{
    public class ViewModelBase : ReactiveObject, IActivatableViewModel
    {
        public static IMessageBus MessageBus => ReactiveUI.MessageBus.Current;
        
        public ViewModelActivator Activator { get; } = new();
    }
}
