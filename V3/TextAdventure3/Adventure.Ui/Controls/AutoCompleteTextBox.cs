using System;
using System.Collections;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using JetBrains.Annotations;

namespace Adventure.Ui.Controls
{
    /// <summary>
    ///     https://www.codeproject.com/Articles/44920/A-Reusable-WPF-Autocomplete-TextBox
    ///     Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///     Step 1a) Using this custom control in a XAML file that exists in the current project.
    ///     Add this XmlNamespace attribute to the root element of the markup file where it is
    ///     to be used:
    ///     xmlns:MyNamespace="clr-namespace:ACTB"
    ///     Step 1b) Using this custom control in a XAML file that exists in a different project.
    ///     Add this XmlNamespace attribute to the root element of the markup file where it is
    ///     to be used:
    ///     xmlns:MyNamespace="clr-namespace:ACTB;assembly=ACTB"
    ///     You will also need to add a project reference from the project where the XAML file lives
    ///     to this project and Rebuild to avoid compilation errors:
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Browse to and select this project]
    ///     Step 2)
    ///     Go ahead and use your control in the XAML file.
    ///     <MyNamespace:AutoCompleteTextBox />
    /// </summary>
    public class AutoCompleteTextBox : TextBox
    {
        // Binding hack - not really necessary.
        //DependencyObject dummy = new DependencyObject();
        private readonly FrameworkElement            _dummy = new();
        private          Func<object, string, bool>? _filter;
        private          ListBox?                    _listBox;
        private          Popup?                      _popup;
        private          bool                        _suppressEvent;
        private          string                      _textCache = "";

        static AutoCompleteTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AutoCompleteTextBox), new FrameworkPropertyMetadata(typeof(AutoCompleteTextBox)));
        }

        [PublicAPI]
        public Func<object, string, bool>? Filter
        {
            get => _filter;
            set
            {
                if (_filter == value) return;
                _filter = value;
                if (_listBox == null) return;
                if (_filter != null)
                    _listBox.Items.Filter = FilterFunc;
                else
                    _listBox.Items.Filter = null;
            }
        }

        private void InternalClosePopup()
        {
            if (_popup != null)
                _popup.IsOpen = false;
        }

        private void InternalOpenPopup()
        {
            if (_popup != null)
                _popup.IsOpen = true;
            if (_listBox != null) _listBox.SelectedIndex = -1;
        }

        [PublicAPI]
        public void ShowPopup()
        {
            if (_listBox == null || _popup == null) InternalClosePopup();
            else if (_listBox.Items.Count == 0) InternalClosePopup();
            else InternalOpenPopup();
        }

        private void SetTextValueBySelection(object? obj, bool moveFocus)
        {
            if (_popup != null)
            {
                InternalClosePopup();
                Dispatcher.Invoke(() =>
                                  {
                                      Focus();
                                      if (moveFocus)
                                          MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                                  }, DispatcherPriority.Background);
            }

            // Retrieve the Binding object from the control.
            var originalBinding = BindingOperations.GetBinding(this, BindingProperty);
            if (originalBinding == null) return;

            // Binding hack - not really necessary.
            //Binding newBinding = new Binding()
            //{
            //    Path = new PropertyPath(originalBinding.Path.Path, originalBinding.Path.PathParameters),
            //    XPath = originalBinding.XPath,
            //    Converter = originalBinding.Converter,
            //    ConverterParameter = originalBinding.ConverterParameter,
            //    ConverterCulture = originalBinding.ConverterCulture,
            //    StringFormat = originalBinding.StringFormat,
            //    TargetNullValue = originalBinding.TargetNullValue,
            //    FallbackValue = originalBinding.FallbackValue
            //};
            //newBinding.Source = obj;
            //BindingOperations.SetBinding(dummy, TextProperty, newBinding);

            // Set the dummy's DataContext to our selected object.
            _dummy.DataContext = obj;

            // Apply the binding to the dummy FrameworkElement.
            BindingOperations.SetBinding(_dummy, TextProperty, originalBinding);
            _suppressEvent = true;

            // Get the binding's resulting value.
            Text           = _dummy.GetValue(TextProperty).ToString();
            _suppressEvent = false;
            if (_listBox != null)
                _listBox.SelectedIndex = -1;
            SelectAll();
        }

        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            base.OnTextChanged(e);
            if (_suppressEvent) return;
            _textCache = Text;
            Debug.Print("Text: " + _textCache);
            if (_popup != null && _textCache == "")
                InternalClosePopup();
            else if (_listBox != null)
            {
                if (_filter != null)
                    _listBox.Items.Filter = FilterFunc;

                if (_popup == null) return;

                if (_listBox.Items.Count == 0)
                    InternalClosePopup();
                else
                    InternalOpenPopup();
            }
        }

        private bool FilterFunc(object obj) => _filter == null || _filter(obj, _textCache);

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _popup   = Template.FindName("PART_Popup", this) as Popup;
            _listBox = Template.FindName("PART_ListBox", this) as ListBox;
            if (_listBox == null) return;

            _listBox.PreviewMouseDown += listBox_MouseUp;
            _listBox.KeyDown          += listBox_KeyDown;
            OnItemsSourceChanged(ItemsSource);
            OnItemTemplateChanged(ItemTemplate);
            OnItemContainerStyleChanged(ItemContainerStyle);
            OnItemTemplateSelectorChanged(ItemTemplateSelector);
            if (_filter != null)
                _listBox.Items.Filter = FilterFunc;
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            if (_suppressEvent) return;
            if (_popup != null)
                InternalClosePopup();
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);
            var fs = FocusManager.GetFocusScope(this);
            var o  = FocusManager.GetFocusedElement(fs);
            switch (e.Key)
            {
                case Key.Escape:
                    InternalClosePopup();
                    Focus();
                    break;
                case Key.Down:
                {
                    if (_listBox != null && Equals(o, this))
                    {
                        _suppressEvent = true;
                        _listBox.Focus();
                        _suppressEvent = false;
                    }

                    break;
                }
            }
        }

        private void listBox_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var dep                                          = (DependencyObject) e.OriginalSource;
            while (dep != null && !(dep is ListBoxItem)) dep = VisualTreeHelper.GetParent(dep);
            if (dep == null) return;
            var item = _listBox?.ItemContainerGenerator.ItemFromContainer(dep);
            if (item == null) return;
            SetTextValueBySelection(item, false);
        }

        private void listBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Return)
                SetTextValueBySelection(_listBox?.SelectedItem, false);
            else if (e.Key == Key.Tab)
                SetTextValueBySelection(_listBox?.SelectedItem, true);
        }

        #region ItemsSource Dependency Property

        [PublicAPI]
        public IEnumerable ItemsSource
        {
            get => (IEnumerable) GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        // Using a DependencyProperty as the backing store for ItemsSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsSourceProperty =
            ItemsControl.ItemsSourceProperty.AddOwner(typeof(AutoCompleteTextBox),
                                                      new UIPropertyMetadata(null, OnItemsSourceChanged));

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is AutoCompleteTextBox actb)) return;
            actb.OnItemsSourceChanged(e.NewValue as IEnumerable);
        }

        [PublicAPI]
        protected void OnItemsSourceChanged(IEnumerable? itemsSource)
        {
            if (_listBox == null) return;
            Debug.Print("Data: " + itemsSource);
            switch (itemsSource)
            {
                case ListCollectionView view:
                    _listBox.ItemsSource = new LimitedListCollectionView((IList) view.SourceCollection) {Limit = MaxCompletions};
                    Debug.Print("Was ListCollectionView");
                    break;
                case CollectionView view:
                    _listBox.ItemsSource = new LimitedListCollectionView(view.SourceCollection) {Limit = MaxCompletions};
                    Debug.Print("Was CollectionView");
                    break;
                case IList list:
                    _listBox.ItemsSource = new LimitedListCollectionView(list) {Limit = MaxCompletions};
                    Debug.Print("Was IList");
                    break;
                default:
                    _listBox.ItemsSource = new LimitedCollectionView(itemsSource) {Limit = MaxCompletions};
                    Debug.Print("Was IEnumerable");
                    break;
            }

            if (_listBox.Items.Count == 0) InternalClosePopup();
        }

        #endregion

        #region Binding Dependency Property

        [PublicAPI]
        public string Binding
        {
            get => (string) GetValue(BindingProperty);
            set => SetValue(BindingProperty, value);
        }

        // Using a DependencyProperty as the backing store for Binding.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BindingProperty =
            DependencyProperty.Register("Binding", typeof(string), typeof(AutoCompleteTextBox), new UIPropertyMetadata(null));

        #endregion

        #region ItemTemplate Dependency Property

        [PublicAPI]
        public DataTemplate ItemTemplate
        {
            get => (DataTemplate) GetValue(ItemTemplateProperty);
            set => SetValue(ItemTemplateProperty, value);
        }

        // Using a DependencyProperty as the backing store for ItemTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemTemplateProperty =
            ItemsControl.ItemTemplateProperty.AddOwner(typeof(AutoCompleteTextBox),
                                                       new UIPropertyMetadata(null, OnItemTemplateChanged));

        private static void OnItemTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is AutoCompleteTextBox actb)) return;
            actb.OnItemTemplateChanged(e.NewValue as DataTemplate);
        }

        private void OnItemTemplateChanged(DataTemplate? p)
        {
            if (_listBox == null) return;
            _listBox.ItemTemplate = p;
        }

        #endregion

        #region ItemContainerStyle Dependency Property

        public Style ItemContainerStyle
        {
            get => (Style) GetValue(ItemContainerStyleProperty);
            set => SetValue(ItemContainerStyleProperty, value);
        }

        // Using a DependencyProperty as the backing store for ItemContainerStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemContainerStyleProperty =
            ItemsControl.ItemContainerStyleProperty.AddOwner(typeof(AutoCompleteTextBox),
                                                             new UIPropertyMetadata(null, OnItemContainerStyleChanged));

        private static void OnItemContainerStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is AutoCompleteTextBox actb)) return;
            actb.OnItemContainerStyleChanged(e.NewValue as Style);
        }

        private void OnItemContainerStyleChanged(Style? p)
        {
            if (_listBox == null) return;
            _listBox.ItemContainerStyle = p;
        }

        #endregion

        #region MaxCompletions Dependency Property

        [PublicAPI]
        public int MaxCompletions
        {
            get => (int) GetValue(MaxCompletionsProperty);
            set => SetValue(MaxCompletionsProperty, value);
        }

        // Using a DependencyProperty as the backing store for MaxCompletions.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxCompletionsProperty =
            DependencyProperty.Register("MaxCompletions", typeof(int), typeof(AutoCompleteTextBox), new UIPropertyMetadata(int.MaxValue));

        #endregion

        #region ItemTemplateSelector Dependency Property

        [PublicAPI]
        public DataTemplateSelector ItemTemplateSelector
        {
            get => (DataTemplateSelector) GetValue(ItemTemplateSelectorProperty);
            set => SetValue(ItemTemplateSelectorProperty, value);
        }

        // Using a DependencyProperty as the backing store for ItemTemplateSelector.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemTemplateSelectorProperty =
            ItemsControl.ItemTemplateSelectorProperty.AddOwner(typeof(AutoCompleteTextBox), new UIPropertyMetadata(null, OnItemTemplateSelectorChanged));

        private static void OnItemTemplateSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is AutoCompleteTextBox actb)) return;
            actb.OnItemTemplateSelectorChanged(e.NewValue as DataTemplateSelector);
        }

        private void OnItemTemplateSelectorChanged(DataTemplateSelector? p)
        {
            if (_listBox == null) return;
            _listBox.ItemTemplateSelector = p;
        }

        #endregion
    }
}