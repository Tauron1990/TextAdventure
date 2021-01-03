using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DynamicData.Binding;
using Tauron;
using Tauron.Application;
using Tauron.Application.CommonUI.Commands;
using Tauron.Application.CommonUI.Dialogs;
using Tauron.Application.Wpf.Dialogs;
using TextAdventure.Editor.Operations.Command;

namespace TextAdventure.Editor.Views.Dialogs
{
    /// <summary>
    /// Interaktionslogik für NewProjectDialog.xaml
    /// </summary>
    public partial class NewProjectDialog : IBaseDialog<TryLoadProjectCommand?, Unit>
    {
        public NewProjectDialog() => InitializeComponent();

        public Task<TryLoadProjectCommand?> Init(Unit initalData) => this.MakeTask<TryLoadProjectCommand?>(t => new NewProjectDialogModel(t));

        private void NewProjectDialog_OnLoaded(object sender, RoutedEventArgs e) => NameBox.Focus();
    }

    public sealed class NewProjectDialogModel : ObservableObject, IDisposable, INotifyDataErrorInfo
    {
        private static readonly string DefaultProjectPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "TextAdventurs Projects");

        public static string ProjectPath() 
            => Directory.Exists(DefaultProjectPath) ? DefaultProjectPath : Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        private readonly CompositeDisposable _disposable = new();
        private readonly ObservableDictionary<string, string> _errors;

        private string? _name;
        private string? _sourcePath;
        private bool _hasErrors;
        private string _pathError = string.Empty;

        public NewProjectDialogModel(IObserver<TryLoadProjectCommand?> waiter)
        {
            _errors = new ObservableDictionary<string, string>();

            InitValidation();

            Cancel = new SimpleReactiveCommand()
                    .Finish(d => d.Subscribe(_ => waiter.OnNext(null)))
                    .DisposeWith(_disposable);

            Create = new SimpleReactiveCommand(this.WhenAny(() => HasErrors).Select(b => !b))
                    .Finish(d => d.Select(_ => new TryLoadProjectCommand(Path.Combine(_sourcePath!, _name!), true, _name!))
                                  .Subscribe(waiter))
                    .DisposeWith(_disposable);

            Name = string.Empty;
            SourcePath = DefaultProjectPath;
        }
        
        private void InitValidation()
        {
            _errors.ObserveCollectionChanges()
                   .Subscribe(_ => SetProperty(ref _hasErrors, _errors.Count != 0, EvaluateErrors, nameof(HasErrors)));

            const string nameEmptyError = "Der GameName darf nicht Leer sein.";
            AddError(this.WhenAny(() => Name).Select(string.IsNullOrWhiteSpace), nameEmptyError, nameof(Name));

            const string sourceEmptyError = "Die Quelle darf nicht Leer sein.";
            AddError(this.WhenAny(() => SourcePath).Select(string.IsNullOrWhiteSpace), sourceEmptyError, nameof(SourcePath));

            const string existingPath = "Der Projekt Ordner Existiert schon";
            var sourceCheck = from name in this.WhenAny(() => Name)
                        from source in this.WhenAny(() => SourcePath)
                        where !string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(source)
                        select !Directory.Exists(Path.Combine(source, name));
            AddError(sourceCheck, existingPath, nameof(Create));
        }

        private void EvaluateErrors()
        {
            PathError = _errors.Values.FirstOrDefault() ?? string.Empty;

            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(nameof(HasErrors)));
        }

        private void AddError(IObservable<bool> observable, string error, string property)
            => observable.Subscribe(b =>
                                    {
                                        if (b)
                                            _errors[property] = error;
                                        else
                                            _errors.Remove(property);
                                    })
                         .DisposeWith(_disposable);

        public string? Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string? SourcePath
        {
            get => _sourcePath;
            set => SetProperty(ref _sourcePath, value);
        }

        public string PathError
        {
            get => _pathError;
            private set => SetProperty(ref _pathError, value);
        }

        public ICommand Cancel { get; }

        public ICommand Create { get; }

        public void Dispose() => _disposable.Dispose();

        public IEnumerable GetErrors(string? propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
                return _errors.Values;

            if (_errors.TryGetValue(propertyName, out var err))
                return err;

            return Array.Empty<string>();
        }

        public bool HasErrors => _hasErrors;

        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;
    }
}
