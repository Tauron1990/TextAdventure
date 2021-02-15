using System.Threading.Tasks;
using System.Windows;
using MaterialDesignExtensions.Controls;
using Tauron.Application.CommonUI.Dialogs;
using Tauron.Application.Wpf.Dialogs;

namespace TextAdventure.Editor.Views.Dialogs
{
    /// <summary>
    ///     Interaktionslogik für GenericOpenDictionaryDialog.xaml
    /// </summary>
    public partial class GenericOpenDictionaryDialog : IBaseDialog<string?, OpenDirectoryDialogArguments>
    {
        private TaskCompletionSource<string?>? _source;

        public GenericOpenDictionaryDialog() => InitializeComponent();

        public Task<string?> Init(OpenDirectoryDialogArguments initalData)
        {
            return this.MakeTask<string?>(t =>
                                          {
                                              _source = t;
                                              return initalData;
                                          });
        }

        private void FileSystemControl_OnCancel(object sender, RoutedEventArgs e) => _source?.SetResult(null);

        private void OpenDirectoryControl_OnDirectorySelected(object sender, RoutedEventArgs e)
            => _source?.SetResult(Selector.CurrentDirectory);
    }
}