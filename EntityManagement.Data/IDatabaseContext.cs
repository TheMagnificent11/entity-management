using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EntityManagement.Data
{
    /// <summary>
    /// Database Context Interface
    /// </summary>
    public interface IDatabaseContext : IDisposable
    {
        /// <summary>
        /// Gets or sets the number of attached repositories
        /// </summary>
        int AttachedRepositories { get; set; }

        /// <summary>
        /// Gets the entity set for the specified entity type
        /// </summary>
        /// <typeparam name="T">Entity type of database set</typeparam>
        /// <returns>Database set</returns>
        DbSet<T> EntitySet<T>()
            where T : class;

        /// <summary>
        /// Gets an <see cref="EntityEntry"/> for the given entity
        /// </summary>
        /// <param name="entity">The entity to get the entry for</param>
        /// <returns>The entry for the given entity</returns>
        EntityEntry Entry(object entity);

        /// <summary>
        /// Begins tracking the given entity such that it will be updated in the database when changes are saved
        /// </summary>
        /// <param name="entity">Entity to update</param>
        /// <returns>The entry for the given entity</returns>
        EntityEntry Update(object entity);

        /// <summary>
        /// Saves all changes made in this context to the underlying database
        /// </summary>
        /// <returns>The number of state entries written to the underlying database</returns>
        Task<int> SaveChangesAsync();
    }
}
