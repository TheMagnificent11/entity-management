using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityManagement.Core;

namespace EntityManagement
{
    /// <summary>
    /// Entity Repository Interface
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    /// <typeparam name="TId">Entity ID type</typeparam>
    public interface IEntityRepository<T, in TId>
        where T : class, IEntity<TId>
        where TId : IComparable
    {
        /// <summary>
        /// Retrieve all
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of entities</returns>
        Task<List<T>> RetrieveAll(CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves the entity with the specified ID
        /// </summary>
        /// <param name="id">ID of entity to retrieve</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Asychronous task containing the entity if it exists, otherwise null</returns>
        Task<T> RetrieveById(TId id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Executes a query on the entity table access by this repository
        /// </summary>
        /// <param name="query">Query specificiation</param>
        /// <returns>Queryable result collection</returns>
        IQueryable<T> Query(IQuerySpecification<T> query);

        /// <summary>
        /// Creates the specified entity
        /// </summary>
        /// <param name="entity">Entity to create</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Asychronous task</returns>
        Task Create(T entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates the specified entity
        /// </summary>
        /// <param name="entity">Entity to update</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Asychronous task</returns>
        Task Update(T entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes the entity with the specified ID
        /// </summary>
        /// <param name="id">ID of entity to delete</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Asychronous task</returns>
        Task Delete(TId id, CancellationToken cancellationToken = default);
    }
}
