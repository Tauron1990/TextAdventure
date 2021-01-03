using Avalonia;
using Avalonia.Markup.Xaml;

namespace TextAdventure.Editor
{
    public class App : Application
    {
        public override void Initialize() 
            => AvaloniaXamlLoader.Load(this);
    }
}