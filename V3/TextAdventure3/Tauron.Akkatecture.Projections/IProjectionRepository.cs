using System.Threading.Tasks;
using Akkatecture.Core;
using JetBrains.Annotations;

namespace Tauron.Akkatecture.Projections
{
    [PublicAPI]
    public interface IProjectionRepository
    {
        Task<TProjection> Get<TProjection, TIdentity>(TIdentity identity)
            where TProjection : IProjectorData<TIdentity>
            where TIdentity : IIdentity;

        Task<TProjection> Create<TProjection, TIdentity>(TIdentity identity)
            where TProjection : IProjectorData<TIdentity>
            where TIdentity : IIdentity;

        Task Commit<TIdentity>(TIdentity identity)
            where TIdentity : IIdentity;
    }
}