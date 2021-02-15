using Tauron.Application.CommonUI;
using TextAdventure.Editor.ViewModels;

namespace TextAdventure.Editor.Views
{
    /// <summary>
    ///     Interaktionslogik für DashboardView.xaml
    /// </summary>
    public partial class DashboardView
    {
        public DashboardView(IViewModel<DashboardViewModel> model)
            : base(model)
        {
            InitializeComponent();
        }
    }
}