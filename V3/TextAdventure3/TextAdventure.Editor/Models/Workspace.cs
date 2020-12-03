using System;
using System.IO;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Newtonsoft.Json;
using TextAdventure.Editor.Models.Data;

namespace TextAdventure.Editor.Models
{
    public sealed class Workspace
    {
        private static readonly ProjectInfo Empty = new(string.Empty, new Version(0, 0), string.Empty, string.Empty);

        public static readonly Workspace Shared = new();


        private readonly BehaviorSubject<ProjectInfo> _projectInfo;

        public IObservable<ProjectInfo> ProjectInfo { get; }

        public IObservable<bool> IsEmpty => ProjectInfo.Select(p => p == Empty);

        private Workspace()
        {
            _projectInfo = new BehaviorSubject<ProjectInfo>(Empty);
            ProjectInfo = _projectInfo.AsObservable();
        }

        public IObservable<string?> Load(IObservable<ProjectValidation> toLoad)
        {
            var validationFail = toLoad.Where(v => !v.IsValid).Select(v => v.Error);

            var loading = toLoad
                         .Where(v => v.IsValid)
                         .Select(DoLoading);

            return validationFail.Merge(loading);
        }

        private string? DoLoading(ProjectValidation project)
        {
            try
            {
                _projectInfo.OnNext(JsonConvert.DeserializeObject<ProjectInfo>(File.ReadAllText(project.ProjectPath)));
                return null;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public void Reset() => _projectInfo.OnNext(Empty);
    }
}