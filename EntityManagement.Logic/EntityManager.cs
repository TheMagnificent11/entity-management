using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EntityManagement.Data;
using EntityManagement.Models;

namespace EntityManagement.Logic
{
    /// <summary>
    /// Entity Manager Interface
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    /// <typeparam name="TId">Entity ID type</typeparam>
    public class EntityManager<T, TId> : IEntityManager<T, TId>
        where T : class, IEntity<TId>
        where TId : IComparable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityManager{T, TId}"/> class
        /// </summary>
        /// <param name="repository">Related entity database repository</param>
        public EntityManager(IEntityRepository<T, TId> repository)
        {
            Repository = repository;
        }

        /// <summary>
        /// Gets the repository
        /// </summary>
        protected IEntityRepository<T, TId> Repository { get; private set; }

        /// <summary>
        /// Retrieves all the entities
        /// </summary>
        /// <returns>A list of the entities</returns>
        public Task<List<T>> RetrieveAll()
        {
            return Repository.RetrieveAll();
        }

        /// <summary>
        /// Retrieves the entity with the specified ID
        /// </summary>
        /// <param name="id">ID of entity to retrieve</param>
        /// <returns>Entity if it exists, otherwise null</returns>
        public Task<T> RetrieveById(TId id)
        {
            return Repository.RetrieveById(id);
        }

        /// <summary>
        /// Creates the specified entity
        /// </summary>
        /// <param name="entity">Entity to create</param>
        /// <returns>Asychronous task</returns>
        public Task Create(T entity)
        {
            return Repository.Create(entity);
        }

        /// <summary>
        /// Updates the specified entity
        /// </summary>
        /// <param name="entity">Entity to update</param>
        /// <returns>Asychronous task</returns>
        public Task Update(T entity)
        {
            return Repository.Update(entity);
        }

        /// <summary>
        /// Deletes the entity with the specified ID
        /// </summary>
        /// <param name="id">ID of entity to delete</param>
        /// <returns>Asychronous task</returns>
        public Task Delete(TId id)
        {
            return Repository.Delete(id);
        }
    }
}
