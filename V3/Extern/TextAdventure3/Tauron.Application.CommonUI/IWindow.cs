namespace Tauron.Application.CommonUI
{
    public interface IWindow
    {
        void Show();
        void Hide();
        bool? ShowDialog();
    }
}