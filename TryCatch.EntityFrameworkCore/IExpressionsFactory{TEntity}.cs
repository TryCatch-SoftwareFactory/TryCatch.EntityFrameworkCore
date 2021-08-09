// <copyright file="IExpressionsFactory{TEntity}.cs" company="TryCatch Software Factory">
// Copyright © TryCatch Software Factory All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>

namespace TryCatch.EntityFrameworkCore
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using Microsoft.EntityFrameworkCore.Query;

    /// <summary>
    /// Factory of EF expression.
    /// </summary>
    /// <typeparam name="TEntity">Type of entities to use on expression.</typeparam>
    public interface IExpressionsFactory<TEntity>
        where TEntity : class
    {
        /// <summary>
        /// Gets a reference to includes expression for a specific query.
        /// </summary>
        /// <param name="queryName">Query name.</param>
        /// <returns>Include expression.</returns>
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> GetIncludesByQuerName(string queryName);

        /// <summary>
        /// Gets a reference to filter expression for a specific query.
        /// </summary>
        /// <param name="queryName">Query name.</param>
        /// <param name="entity">Reference to the entity to filter(optional).</param>
        /// <returns>Filter expression.</returns>
        Expression<Func<TEntity, bool>> GetWhereByQueryName(string queryName, TEntity entity = default);

        /// <summary>
        /// Gets a reference to sort by expression for a specific query.
        /// </summary>
        /// <param name="queryName">Query name.</param>
        /// <returns>SortBy expression.</returns>
        Expression<Func<TEntity, object>> GetSortByByQueryName(string queryName);
    }
}
