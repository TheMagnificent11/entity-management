namespace EntityManagement.Core
{
    /// <summary>
    /// Soft Delete Entity Interface
    /// </summary>
    public interface ISoftDeleteEntity
    {
        /// <summary>
        /// Gets or sets a value indicating whether the entity is deleted
        /// </summary>
        bool IsDeleted { get; set; }
    }
}
