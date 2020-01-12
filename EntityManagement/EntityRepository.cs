using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityManagement.Core;
using Microsoft.EntityFrameworkCore;

namespace EntityManagement
{
    /// <summary>
    /// Entity Repository
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TId">Entity Id type</typeparam>
    public class EntityRepository<TEntity, TId> : IEntityRepository<TEntity, TId>, IDisposable
        where TEntity : class, IEntity<TId>
        where TId : IComparable, IComparable<TId>, IEquatable<TId>, IConvertible
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityRepository{TEntity,TId}" /> class
        /// </summary>
        /// <param name="context">Database context</param>
        public EntityRepository(IDatabaseContext context)
        {
            this.Context = context ?? throw new ArgumentNullException(nameof(context));
            this.Context.AttachedRepositories++;
        }

        /// <summary>
        /// Gets the database context
        /// </summary>
        protected IDatabaseContext Context { get; private set; }

        private bool Disposed { get; set; }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Retrieves all the entities
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>A list of the entities</returns>
        public Task<List<TEntity>> RetrieveAll(CancellationToken cancellationToken = default)
        {
            return this.Context.EntitySet<TEntity>()
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Retrieves the entity with the specified ID
        /// </summary>
        /// <param name="id">ID of entity to retrieve</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Entity if it exists, otherwise null</returns>
        public Task<TEntity> RetrieveById(TId id, CancellationToken cancellationToken = default)
        {
            return this.Context.EntitySet<TEntity>()
                .SingleOrDefaultAsync(i => i.Id.CompareTo(id) == 0, cancellationToken);
        }

        /// <summary>
        /// Executes a query on the entity table access by this repository
        /// </summary>
        /// <param name="query">Query specificiation</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Asychronous task containing the query result collection</returns>
        public Task<List<TEntity>> Query(
            IQuerySpecification<TEntity> query,
            CancellationToken cancellationToken = default)
        {
            if (query == null) throw new ArgumentNullException(nameof(query));
            if (query.Criteria == null) throw new ArgumentException(nameof(query.Criteria));

            if (query.Includes == null)
            {
                return this.Context.EntitySet<TEntity>()
                    .Where(query.Criteria)
                    .ToListAsync(cancellationToken);
            }
            else
            {
                var queryableResultWithIncludes = query.Includes
                    .Aggregate(
                        this.Context.EntitySet<TEntity>().AsQueryable(),
                        (current, include) => current.Include(include));

                return queryableResultWithIncludes
                    .Where(query.Criteria)
                    .ToListAsync(cancellationToken);
            }
        }

        /// <summary>
        /// Creates the specified entity
        /// </summary>
        /// <param name="entity">Entity to create</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Asychronous task</returns>
        public async Task Create(TEntity entity, CancellationToken cancellationToken = default)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            this.Context.EntitySet<TEntity>().Add(entity);

            await this.Context.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Updates the specified entity
        /// </summary>
        /// <param name="entity">Entity to update</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Asychronous task</returns>
        public async Task Update(TEntity entity, CancellationToken cancellationToken = default)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            var existing = await this.RetrieveById(entity.Id);
            if (existing == null) return;

            this.Context.Entry(existing).CurrentValues.SetValues(entity);

            await this.Context.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Delete the entity with the specified ID
        /// </summary>
        /// <param name="id">ID of entity to delete</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Task to enable asynchronous execution</returns>
        public async Task Delete(TId id, CancellationToken cancellationToken = default)
        {
            var entity = await this.RetrieveById(id);
            if (entity == null) return;

            this.Context.EntitySet<TEntity>().Remove(entity);
            await this.Context.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Releases the unmanaged resources that are used by the object and, optionally, releases the managed resources
        /// </summary>
        /// <param name="disposing">
        /// True to release both managed and unmanaged resources; false to release only unmanaged resources
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (this.Disposed) return;
            if (!disposing) return;

            if (this.Context != null)
            {
                this.Context.AttachedRepositories--;

                if (this.Context.AttachedRepositories == 0 && this.Context is IDisposable)
                {
                    this.Context.Dispose();
                    this.Context = null;
                }
            }

            this.Disposed = true;
        }
    }
}
