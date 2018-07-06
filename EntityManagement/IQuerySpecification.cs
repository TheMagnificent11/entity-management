using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace EntityManagement
{
    /// <summary>
    /// Query Specification Interface
    /// </summary>
    /// <typeparam name="T">Entity type being queried</typeparam>
    public interface IQuerySpecification<T>
        where T : class
    {
        /// <summary>
        /// Gets the query crieteria
        /// </summary>
        Expression<Func<T, bool>> Criteria { get; }

        /// <summary>
        /// Gets the query includes
        /// </summary>
        List<Expression<Func<T, object>>> Includes { get; }
    }
}
