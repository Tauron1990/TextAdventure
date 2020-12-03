using System;
using System.IO;
using System.Reactive.Linq;
using TextAdventure.Editor.IO;

namespace TextAdventure.Editor.Models.Data
{
    public sealed record ProjectValidation(bool IsValid, string Error, string ProjectPath)
    {
        public const string ProjectFileName = "TextAdventure.proj";

        public static IObservable<ProjectValidation> Validate(IObservable<string> input)
            => input
              .Select(s => Path.Combine(s, ProjectFileName))
              .SafeFileExists()
              .Map((exists, path) => new ProjectValidation(exists, exists
                                                                       ? "Datei nicht gefunden"
                                                                       : string.Empty, path),
                   (exception, path) => new ProjectValidation(false, exception.Message, path));
    }
}