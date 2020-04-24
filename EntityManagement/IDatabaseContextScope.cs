using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace EntityManagement
{
    /// <summary>
    /// Database Context Inteface
    /// </summary>
    /// <typeparam name="TContext">Datbase Context type</typeparam>
    public interface IDatabaseContextScope<TContext>
        where TContext : IDatabaseContext
    {
        /// <summary>
        /// Gets the entity set for the specified entity type
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <returns>Database set</returns>
        DbSet<T> EntitySet<T>()
            where T : class;

        /// <summary>
        /// Executes a query on the entity table access by this repository
        /// </summary>
        /// <param name="query">Query specificiation</param>
        /// <typeparam name="T">Entity type</typeparam>
        /// <returns>Queryable result collection</returns>
        IQueryable<T> Query<T>(IQuerySpecification<T> query)
            where T : class;

        /// <summary>
        /// Saves all changes made in this context to the underlying database
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The number of state entries written to the underlying database</returns>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
