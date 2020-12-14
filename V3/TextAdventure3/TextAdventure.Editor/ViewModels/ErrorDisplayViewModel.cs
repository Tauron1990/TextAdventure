using System;
using ReactiveUI;
using TextAdventure.Editor.ViewModels.Shared;

namespace TextAdventure.Editor.ViewModels
{
    public sealed class ErrorDisplayViewModel : ViewModelBase, IDisposable
    {
        private readonly IDisposable _errorSubscription;
        private string _errorContent = string.Empty;

        public string ErrorContent
        {
            get => _errorContent;
            set => this.RaiseAndSetIfChanged(ref _errorContent, value);
        }

        public ErrorDisplayViewModel() 
            => _errorSubscription = MessageBus.Listen<PropagateError>().Subscribe(e => ErrorContent = e.Error);

        public void Dispose() => _errorSubscription.Dispose();
    }
}