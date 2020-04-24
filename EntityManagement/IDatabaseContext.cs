using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace EntityManagement
{
    /// <summary>
    /// Database Context Interface
    /// </summary>
    public interface IDatabaseContext
    {
        /// <summary>
        /// Gets the entity set for the specified entity type
        /// </summary>
        /// <typeparam name="T">Entity type of database set</typeparam>
        /// <returns>Database set</returns>
        DbSet<T> EntitySet<T>()
            where T : class;

        /// <summary>
        /// Saves all changes made in this context to the underlying database
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The number of state entries written to the underlying database</returns>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
