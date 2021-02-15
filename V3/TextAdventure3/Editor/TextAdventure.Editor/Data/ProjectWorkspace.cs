using System;
using System.Threading;
using System.Threading.Tasks;
using Tauron.Application.Workshop.Mutation;
using Tauron.Application.Workshop.StateManagement.Attributes;
using Tauron.Application.Workshop.StateManagement.DataFactorys;
using TextAdventure.Editor.Data.PartialData;

namespace TextAdventure.Editor.Data
{
    [DataSource]
    public sealed class ProjectWorkspace : MapSourceFactory
    {
        private readonly SemaphoreSlim _semaphore = new(1, 1);
        private IOData _currentProject = new();

        public ProjectWorkspace()
        {
            Register<IOSource, IOData>(() => new IOSource(_semaphore, () => _currentProject,
                                           data => _currentProject = data));
            Register<CommonDataSource, CommonData>(() => new CommonDataSource(_semaphore, () => _currentProject,
                                                       data => _currentProject = data));
        }

        private abstract class SyncSource<TData> : IExtendedDataSource<TData>
        {
            private readonly Func<IOData> _provider;
            private readonly SemaphoreSlim _semaphore;
            private readonly Action<IOData> _updater;

            protected SyncSource(SemaphoreSlim semaphore, Func<IOData> provider, Action<IOData> updater)
            {
                _semaphore = semaphore;
                _provider = provider;
                _updater = updater;
            }

            public async Task<TData> GetData(IQuery query)
            {
                await _semaphore.WaitAsync();
                return await GetDataImpl(query, _provider());
            }

            public async Task SetData(IQuery query, TData data)
                => _updater(await SetDataImpl(query, data, _provider()));

            public async Task OnCompled(IQuery query)
            {
                try
                {
                    await OnCompledImpl(query);
                }
                finally
                {
                    _semaphore.Release();
                }
            }

            protected abstract Task<TData> GetDataImpl(IQuery query, IOData project);

            protected abstract Task<IOData> SetDataImpl(IQuery query, TData data, IOData project);

            protected virtual Task OnCompledImpl(IQuery query) => Task.CompletedTask;
        }

        private sealed class IOSource : SyncSource<IOData>
        {
            public IOSource(SemaphoreSlim semaphore, Func<IOData> provider, Action<IOData> updater) : base(semaphore,
                provider, updater)
            {
            }

            protected override Task<IOData> SetDataImpl(IQuery query, IOData data, IOData project)
                => Task.FromResult(data);

            protected override Task<IOData> GetDataImpl(IQuery query, IOData project) => Task.FromResult(project);
        }

        private sealed class CommonDataSource : SyncSource<CommonData>
        {
            public CommonDataSource(SemaphoreSlim semaphore, Func<IOData> provider, Action<IOData> updater)
                : base(semaphore, provider, updater)
            {
            }

            protected override Task<CommonData> GetDataImpl(IQuery query, IOData project)
                => Task.FromResult(new CommonData(project.Project.GameName, project.Project.GameVersion));

            protected override Task<IOData> SetDataImpl(IQuery query, CommonData data, IOData project)
                => Task.FromResult(project with
                                   {
                                       Project = project.Project with
                                                 {
                                                     GameName = data.Name,
                                                     GameVersion = data.Version
                                                 }
                                   });
        }
    }
}