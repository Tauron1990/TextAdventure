using System;
using System.IO;
using System.Reactive.Linq;
using Newtonsoft.Json;
using Tauron;
using Tauron.Application.Workshop.Mutating;
using Tauron.Application.Workshop.Mutating.Changes;
using Tauron.Application.Workshop.StateManagement.Attributes;
using TextAdventure.Editor.Data.PartialData;
using TextAdventure.Editor.Data.ProjectData;
using TextAdventure.Editor.Operations.Command;
using TextAdventure.Editor.Operations.Events;

namespace TextAdventure.Editor.Operations.Process
{
    [BelogsToState(typeof(IOState))]
    public static class IOProcessor
    {
        private const string ProjectFileName = "Game.proj";

        [Reducer]
        public static IObservable<MutatingContext<IOData>> TrySave(IObservable<MutatingContext<IOData>> input,
            SaveCommand command)
        {
            return input.Select(c =>
                                {
                                    try
                                    {
                                        if (!File.Exists(c.Data.SourcePath))
                                            return c.WithChange(
                                                new ProjectSaveEvent(false, "Die Datei Existiert nicht"));

                                        File.WriteAllText(c.Data.SourcePath,
                                            JsonConvert.SerializeObject(c.Data.Project));
                                        return c.WithChange(new ProjectSaveEvent(true, string.Empty));
                                    }
                                    catch (Exception e)
                                    {
                                        return c.WithChange(new ProjectSaveEvent(false, e.Message));
                                    }
                                });
        }

        [Reducer]
        public static IObservable<MutatingContext<IOData>> TryLoad(IObservable<MutatingContext<IOData>> input,
            TryLoadProjectCommand command)
        {
            var result =
                command.IsNew
                    ? input.SelectSafe(context =>
                                       {
                                           if (Directory.Exists(command.TargetPath))
                                               return (context, new LoadFailedEvent("Der Ordner Existiert Schon"));

                                           Directory.CreateDirectory(command.TargetPath);
                                           var path = Path.Combine(command.TargetPath, ProjectFileName);
                                           var gameData = GameProject.Create(command.Name, new Version(0, 1));

                                           File.WriteAllText(path, JsonConvert.SerializeObject(gameData));

                                           return (context, (MutatingChange) new ProjectLoadedEvent(gameData, path));
                                       })
                    : input.SelectSafe(context =>
                                       {
                                           MutatingChange change;

                                           if (Directory.Exists(command.TargetPath))
                                           {
                                               string realPath = Path.Combine(command.TargetPath, ProjectFileName);
                                               if (File.Exists(realPath))
                                               {
                                                   var data = JsonConvert.DeserializeObject<GameProject>(
                                                       File.ReadAllText(realPath));
                                                   change = new ProjectLoadedEvent(data, realPath);
                                               }
                                               else
                                               {
                                                   change = new LoadFailedEvent("Die Projekt Datei Existiert nicht");
                                               }
                                           }
                                           else
                                           {
                                               change = new LoadFailedEvent("Der Projekt Ordner Existiert nicht");
                                           }

                                           return (context, change);
                                       });

            return result.ConvertResult(dat => dat.context.WithChange(dat.Item2),
                exception => MutatingContext<IOData>.New(new IOData())
                                                    .WithChange(new LoadFailedEvent(exception.Message)));
        }
    }
}