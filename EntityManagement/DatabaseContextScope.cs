using System;
using System.Linq;
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
        /// Executes a query on the entity table access by this repository
        /// </summary>
        /// <param name="query">Query specificiation</param>
        /// <typeparam name="T">Entity type</typeparam>
        /// <returns>Queryable result collection</returns>
        public IQueryable<T> Query<T>(IQuerySpecification<T> query)
            where T : class
        {
            if (query == null) throw new ArgumentNullException(nameof(query));
            if (query.Criteria == null) throw new ArgumentException(nameof(query.Criteria));

            if (query.Includes == null)
            {
                return this.context.EntitySet<T>()
                    .Where(query.Criteria);
            }
            else
            {
                var queryableResultWithIncludes = query.Includes
                    .Aggregate(
                        this.context.EntitySet<T>().AsQueryable(),
                        (current, include) => current.Include(include));

                return queryableResultWithIncludes
                    .Where(query.Criteria);
            }
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
