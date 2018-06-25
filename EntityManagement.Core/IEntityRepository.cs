using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EntityManagement.Core
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
        /// <returns>List of entities</returns>
        Task<List<T>> RetrieveAll();

        /// <summary>
        /// Retrieves the entity with the specified ID
        /// </summary>
        /// <param name="id">ID of entity to retrieve</param>
        /// <returns>Asychronous task containing the entity if it exists, otherwise null</returns>
        Task<T> RetrieveById(TId id);

        /// <summary>
        /// Creates the specified entity
        /// </summary>
        /// <param name="entity">Entity to create</param>
        /// <returns>Asychronous task</returns>
        Task Create(T entity);

        /// <summary>
        /// Updates the specified entity
        /// </summary>
        /// <param name="entity">Entity to update</param>
        /// <returns>Asychronous task</returns>
        Task Update(T entity);

        /// <summary>
        /// Deletes the entity with the specified ID
        /// </summary>
        /// <param name="id">ID of entity to delete</param>
        /// <returns>Asychronous task</returns>
        Task Delete(TId id);
    }
}
