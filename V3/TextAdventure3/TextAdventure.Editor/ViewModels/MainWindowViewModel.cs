using System;
using System.Collections.Generic;
using System.Text;
using Avalonia.Layout;
using DynamicData.Binding;
using ReactiveUI;

namespace TextAdventure.Editor.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public IReactiveCommand OpenCommand { get; }

        public MainWindowViewModel()
        {

            OpenCommand = new ReactiveCommand<object, object>();
        }
    }
}
