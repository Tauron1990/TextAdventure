using System;
using System.IO;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using LibGit2Sharp;
using Newtonsoft.Json;
using TextAdventure.Editor.Models.Data;
using Version = System.Version;

namespace TextAdventure.Editor.Models
{
    public sealed record SetupProject(string ProjectName, string GameName, string ProjectPath, bool InitGit);
    
    public sealed class Workspace
    {
        private static readonly ProjectInfo Empty = new(string.Empty, new Version(0, 0), string.Empty, string.Empty);

        public static readonly Workspace Shared = new();


        private readonly BehaviorSubject<ProjectInfo> _projectInfo;

        public IObservable<ProjectInfo> ProjectReset => _projectInfo.AsObservable();
        
        public IObservable<ProjectInfo> ProjectInfo => _projectInfo.Where(p => p != Empty);

        public IObservable<bool> IsEmpty => _projectInfo.Select(p => p == Empty);

        private Workspace() => _projectInfo = new BehaviorSubject<ProjectInfo>(Empty);

        public IObservable<string?> Load(IObservable<ProjectValidation> toLoad) 
            => toLoad.Select(pv => pv.IsValid ? DoLoading(pv) : pv.Error);

        private string? DoLoading(ProjectValidation project)
        {
            try
            {
                _projectInfo.OnNext(JsonConvert.DeserializeObject<ProjectInfo>(File.ReadAllText(project.ProjectPath))
                                    with{ RootPath = Path.GetDirectoryName(project.ProjectPath) ?? throw new InvalidOperationException("projekt Verzeichnis nicht gefunden")});
                return null;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public void Reset() => _projectInfo.OnNext(Empty);

        public IObservable<string?> Setup(SetupProject setup, Func<IObservable<string>, IObservable<bool>> overridePath)
        {
            IObservable<bool> lastCheck = Directory.Exists(setup.ProjectPath) 
                                              ? overridePath(Observable.Start(() => setup.ProjectPath)) 
                                              : Observable.Start(() => true);

            return Load(ProjectValidation.Validate(MakeSetupProcess(setup, lastCheck)));
        }

        private IObservable<string> MakeSetupProcess(SetupProject setup, IObservable<bool> lastCheck)
        {
            string CreateBasicData(ProjectInfo info)
            {
                if(Directory.Exists(setup.ProjectPath))
                    Directory.Delete(setup.ProjectPath, true);
                Directory.CreateDirectory(setup.ProjectPath);

                string fullPath = Path.Combine(setup.ProjectPath, ProjectValidation.ProjectFileName);
                File.WriteAllText(fullPath, JsonConvert.SerializeObject(info));

                fullPath = Path.Combine(setup.ProjectPath, info.ContentFile);
                File.WriteAllText(fullPath, JsonConvert.SerializeObject(new ProjectContentFile()));

                if (setup.InitGit)
                    Repository.Init(setup.ProjectPath);
                
                return setup.ProjectPath;
            }
            
            return from isOk in lastCheck
                   where isOk
                   let info = new ProjectInfo(setup.GameName, new Version(0, 1), setup.ProjectName, "Content.json") { RootPath = setup.ProjectPath }
                   select CreateBasicData(info);
        }
    }
}