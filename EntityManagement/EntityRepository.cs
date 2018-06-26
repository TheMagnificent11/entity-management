using System;
using System.Collections.Generic;
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
            Context = context;
            Context.AttachedRepositories++;
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
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Retrieves all the entities
        /// </summary>
        /// <returns>A list of the entities</returns>
        public Task<List<TEntity>> RetrieveAll()
        {
            return Context.EntitySet<TEntity>()
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves the entity with the specified ID
        /// </summary>
        /// <param name="id">ID of entity to retrieve</param>
        /// <returns>Entity if it exists, otherwise null</returns>
        public Task<TEntity> RetrieveById(TId id)
        {
            return Context.EntitySet<TEntity>()
                .SingleOrDefaultAsync(i => i.Id.CompareTo(id) == 0);
        }

        /// <summary>
        /// Creates the specified entity
        /// </summary>
        /// <param name="entity">Entity to create</param>
        /// <returns>Asychronous task</returns>
        public async Task Create(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            Context.EntitySet<TEntity>().Add(entity);

            await Context.SaveChangesAsync();
        }

        /// <summary>
        /// Updates the specified entity
        /// </summary>
        /// <param name="entity">Entity to update</param>
        /// <returns>Asychronous task</returns>
        public async Task Update(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            var existing = await RetrieveById(entity.Id);
            if (existing == null) return;

            Context.Entry(existing).CurrentValues.SetValues(entity);

            await Context.SaveChangesAsync();
        }

        /// <summary>
        /// Delete the entity with the specified ID
        /// </summary>
        /// <param name="id">ID of entity to delete</param>
        /// <returns>Task to enable asynchronous execution</returns>
        public async Task Delete(TId id)
        {
            var entity = await RetrieveById(id);
            if (entity == null) return;

            Context.EntitySet<TEntity>().Remove(entity);
            await Context.SaveChangesAsync();
        }

        /// <summary>
        /// Releases the unmanaged resources that are used by the object and, optionally, releases the managed resources
        /// </summary>
        /// <param name="disposing">
        /// True to release both managed and unmanaged resources; false to release only unmanaged resources
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (Disposed) return;
            if (!disposing) return;

            if (Context != null)
            {
                Context.AttachedRepositories--;

                if (Context.AttachedRepositories == 0 && Context is IDisposable)
                {
                    Context.Dispose();
                    Context = null;
                }
            }

            Disposed = true;
        }
    }
}
