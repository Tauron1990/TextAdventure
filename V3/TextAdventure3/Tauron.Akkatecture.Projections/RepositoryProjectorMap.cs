using System;
using System.Threading.Tasks;
using LiquidProjections;
using LiquidProjections.Statistics;

namespace Tauron.Akkatecture.Projections
{
    public class RepositoryProjectorMap<TProjection, TIdentity>
    {
        private readonly IProjectionRepository _repository;

        protected internal ProjectorMap<TProjection, TIdentity, ProjectionContext> ProjectorMap;

        public RepositoryProjectorMap(IProjectionRepository repository)
        {
            _repository = repository;
            ProjectorMap = new ProjectorMap<TProjection, TIdentity, ProjectionContext>
            {
                Create = Create,
                Delete = Delete,
                Update = Update,
                Custom = Custom
            };
        }

        protected virtual Task Custom(ProjectionContext context, Func<Task> projector)
        {
            
        }

        protected virtual Task Update(TIdentity key, ProjectionContext context, Func<TProjection, Task> projector, Func<bool> createifmissing)
        {
            
        }

        protected virtual Task<bool> Delete(TIdentity key, ProjectionContext context)
        {
            
        }

        protected virtual Task Create(TIdentity key, ProjectionContext context, Func<TProjection, Task> projector, Func<TProjection, bool> shouldoverwite)
        {
            
        }
    }
}