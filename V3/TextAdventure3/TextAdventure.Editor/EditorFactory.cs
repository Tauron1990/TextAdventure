using Dock.Model;
using Dock.Model.Controls;

namespace TextAdventure.Editor
{
    public sealed class EditorFactory : Dock.Model.Factory
    {
        public override IDock CreateLayout() => new RootDock();
    }
}