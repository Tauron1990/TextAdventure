using System.Windows;
using System.Windows.Markup;

[assembly: XmlnsDefinition("http://schemas.TauronWpf.com", "Tauron.Application.Wpf")]
[assembly: XmlnsDefinition("http://schemas.TauronWpf.com", "Tauron.Application.Wpf.Converter")]
[assembly: XmlnsDefinition("http://schemas.TauronWpf.com", "Tauron.Application.Wpf.Model")]
[assembly: XmlnsDefinition("http://schemas.TauronWpf.com", "Tauron.Application.Wpf.UI")]
[assembly: XmlnsDefinition("http://schemas.TauronWpf.com", "Tauron.Application.Wpf.Dialogs")]

[assembly: ThemeInfo(
    ResourceDictionaryLocation.None, //where theme specific resource dictionaries are located
    //(used if a resource is not found in the page,
    // or application resource dictionaries)
    ResourceDictionaryLocation.SourceAssembly //where the generic resource dictionary is located
    //(used if a resource is not found in the page,
    // app, or any theme specific resource dictionaries)
)]