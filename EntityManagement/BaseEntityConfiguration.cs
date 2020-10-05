﻿using System;
using EntityManagement.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EntityManagement
{
    /// <summary>
    /// Base Entity Configuration
    /// </summary>
    /// <typeparam name="TEntity">Entity type to be configured</typeparam>
    /// <typeparam name="TId">Entity key type</typeparam>
    public abstract class BaseEntityConfiguration<TEntity, TId> : IEntityTypeConfiguration<TEntity>
        where TEntity : class, IEntity<TId>
    {
        /// <summary>
        /// Configures the entity of type <see cref="IEntity{TId}"/>
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity type</param>
        public virtual void Configure(EntityTypeBuilder<TEntity> builder)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.HasKey(i => i.Id);

            builder.Property(i => i.CreatedBy)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(i => i.ModifiedBy)
                .IsRequired()
                .HasMaxLength(255);
        }
    }
}
