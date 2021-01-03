using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace TextAdventure.Editor.Views
{
    public class QuestionBox : Window
    {
        public QuestionBox()
        {
            InitializeComponent();
        }
        
        public QuestionBox(string title, string text)
        {
            InitializeComponent();

            Title = title;
            this.FindControl<TextBlock>("MainText").Text = text;
            this.FindControl<Button>("Cancel").Click += CancelClick;
            this.FindControl<Button>("Next").Click += NextClick;
        }

        private void NextClick(object? sender, RoutedEventArgs e) => Close(true);

        private void CancelClick(object? sender, RoutedEventArgs e) => Close(false);

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
