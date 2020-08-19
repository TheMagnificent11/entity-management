using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace EntityManagement
{
    /// <summary>
    /// Database Context Scope
    /// </summary>
    /// <typeparam name="TContext">Database Context type</typeparam>
    public class DatabaseContextScope<TContext> : IDatabaseContextScope<TContext>
        where TContext : IDatabaseContext
    {
        private readonly TContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseContextScope{TContext}"/> class
        /// </summary>
        /// <param name="context">Database context</param>
        public DatabaseContextScope(TContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Gets the entity set for the specified entity type
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <returns>Database set</returns>
        public DbSet<T> EntitySet<T>()
            where T : class
        {
            return this.context.EntitySet<T>();
        }

        /// <summary>
        /// Saves all changes made in this context to the underlying database
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The number of state entries written to the underlying database</returns>
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return this.context.SaveChangesAsync(cancellationToken);
        }
    }
}
