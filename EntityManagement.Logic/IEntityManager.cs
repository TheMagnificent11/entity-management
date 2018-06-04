using System.Collections.Generic;
using System.Threading.Tasks;
using EntityManagement.Models;

namespace EntityManagement.Logic
{
    /// <summary>
    /// Entity Manager Interface
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TId">Entity ID type</typeparam>
    public interface IEntityManager<TEntity, TId>
        where TEntity : IEntity<TId>
    {
        /// <summary>
        /// Retrieves all the entities
        /// </summary>
        /// <returns>A list of the entities</returns>
        Task<List<TEntity>> RetrieveAll();

        /// <summary>
        /// Retrieve an entity using its ID
        /// </summary>
        /// <param name="id">ID of entity to retrieve</param>
        /// <returns>An entity if it exists, otherwise null</returns>
        Task<TEntity> RetrieveById(TId id);

        /// <summary>
        /// Creates an entity
        /// </summary>
        /// <param name="entity">Entity to create</param>
        /// <returns>Task to enable asynchronous execution</returns>
        Task Create(TEntity entity);

        /// <summary>
        /// Updates an entity
        /// </summary>
        /// <param name="entity">Entity to create</param>
        /// <returns>Task to enable asynchronous execution</returns>
        Task Update(TEntity entity);

        /// <summary>
        /// Deletes the entity with the specified ID
        /// </summary>
        /// <param name="id">ID of entity to delete</param>
        /// <returns>Task to enable asynchronous execution</returns>
        Task Delete(TId id);
    }
}
