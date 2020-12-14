using Avalonia.Controls;

namespace TextAdventure.Editor
{
    public sealed class ViewFactory<TView, TModel>
        where TView : Control
    {
        public TView View { get; }

        public ViewFactory(TView view, TModel model)
        {
            View = view;
            view.DataContext = model;
        }
    }
}