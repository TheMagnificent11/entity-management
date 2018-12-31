using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using EntityManagement.Abstractions;

namespace EntityManagement
{
    /// <summary>
    /// Base Query Specification
    /// </summary>
    /// <typeparam name="T">Query entity type</typeparam>
    public abstract class BaseQuerySpecification<T> : IQuerySpecification<T>
        where T : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseQuerySpecification{T}"/> class
        /// </summary>
        /// <param name="criteria">Query criteria</param>
        protected BaseQuerySpecification(Expression<Func<T, bool>> criteria)
        {
            Criteria = criteria;
            Includes = new List<Expression<Func<T, object>>>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseQuerySpecification{T}"/> class
        /// </summary>
        /// <param name="criteria">Query criteria</param>
        /// <param name="includes">Query includes</param>
        protected BaseQuerySpecification(
            Expression<Func<T, bool>> criteria,
            List<Expression<Func<T, object>>> includes)
        {
            Criteria = criteria;
            Includes = includes;
        }

        /// <summary>
        /// Gets the query crieteria
        /// </summary>
        public Expression<Func<T, bool>> Criteria { get; private set; }

        /// <summary>
        /// Gets the query includes
        /// </summary>
        public List<Expression<Func<T, object>>> Includes { get; private set; }

        /// <summary>
        /// Adds an include expression
        /// </summary>
        /// <param name="includeExpression">Include expression to include</param>
        protected void AddInclude(Expression<Func<T, object>> includeExpression)
        {
            Includes.Add(includeExpression);
        }
    }
}
