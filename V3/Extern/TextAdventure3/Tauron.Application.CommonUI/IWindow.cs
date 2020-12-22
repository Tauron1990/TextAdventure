namespace Tauron.Application.CommonUI
{
    public interface IWindow : IUIElement
    {
        void Show();
        void Hide();
        bool? ShowDialog();
    }
}