// <copyright file="QueryRepository{TEntity,TDbContext}.cs" company="TryCatch Software Factory">
// Copyright © TryCatch Software Factory All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>

namespace TryCatch.EntityFrameworkCore.Linq
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Query;
    using TryCatch.Patterns.Repositories;
    using TryCatch.Validators;

    /// <summary>
    /// Abstract implementation of Linq Query Repository. Allows working with NO TRACKING entities.
    /// </summary>
    /// <typeparam name="TEntity">Type of entity.</typeparam>
    /// <typeparam name="TDbContext">Type of DbContext.</typeparam>
    public abstract class QueryRepository<TEntity, TDbContext> : ILinqQueryRepository<TEntity>, IDisposable
        where TEntity : class
        where TDbContext : DbContext
    {
        private const int DefaultLimit = 1000;

        private bool disposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryRepository{TEntity, TDbContext}"/> class.
        /// </summary>
        /// <param name="context">Reference to DbContext.</param>
        /// <param name="expressionsFactory">Reference to expressions factory.</param>
        protected QueryRepository(TDbContext context, IExpressionsFactory<TEntity> expressionsFactory)
        {
            ArgumentsValidator.ThrowIfIsNull(context);
            ArgumentsValidator.ThrowIfIsNull(expressionsFactory);

            this.Context = context;
            this.ExpressionsFactory = expressionsFactory;
        }

        protected TDbContext Context { get; }

        protected IQueryable<TEntity> Entities => this.Context.Set<TEntity>().AsNoTracking();

        protected IExpressionsFactory<TEntity> ExpressionsFactory { get; }

        /// <inheritdoc />
        public void Dispose()
        {
            // Dispose of unmanaged resources.
            this.Dispose(true);

            // Suppress finalization.
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc/>
        public async Task<TEntity> GetAsync(
            Expression<Func<TEntity, bool>> where,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            ArgumentsValidator.ThrowIfIsNull(where);

            var includes = this.ExpressionsFactory.GetIncludesByQuerName(QueriesNames.DefaultGet);

            return await this.GetAsync(where, includes, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<long> GetCountAsync(
            Expression<Func<TEntity, bool>> where = null,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            where ??= this.ExpressionsFactory.GetWhereByQueryName(QueriesNames.DefaultCount);

            return await this.Entities
                .LongCountAsync(where, cancellationToken)
                .ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<TEntity>> GetPageAsync(
            int offset = 1,
            int limit = DefaultLimit,
            Expression<Func<TEntity, bool>> where = null,
            Expression<Func<TEntity, object>> orderBy = null,
            bool orderAsAscending = true,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            ArgumentsValidator.ThrowIfIsLessThan(1, offset, $"Offset value is invalid: {offset}");
            ArgumentsValidator.ThrowIfIsLessThan(1, limit, $"Limit value is invalid: {limit}");

            var includes = this.ExpressionsFactory.GetIncludesByQuerName(QueriesNames.DefaultPage);
            where ??= this.ExpressionsFactory.GetWhereByQueryName(QueriesNames.DefaultPage);
            orderBy ??= this.ExpressionsFactory.GetSortByByQueryName(QueriesNames.DefaultPage);

            return await this
                .GetPageAsync(offset, limit, where, orderBy, includes, orderAsAscending, cancellationToken)
                .ConfigureAwait(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                this.Context.Dispose();
            }

            this.disposed = true;
        }

        protected async virtual Task<TEntity> GetAsync(
            Expression<Func<TEntity, bool>> where,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includes,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var query = includes is null ? this.Entities : includes(this.Entities);

            return await query
                .FirstOrDefaultAsync(where, cancellationToken)
                .ConfigureAwait(false);
        }

        protected async virtual Task<IEnumerable<TEntity>> GetPageAsync(
            int offset,
            int limit,
            Expression<Func<TEntity, bool>> where,
            Expression<Func<TEntity, object>> orderBy,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includes,
            bool orderAsAscending,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            ArgumentsValidator.ThrowIfIsNull(where, "Where expression is null");
            ArgumentsValidator.ThrowIfIsNull(orderBy, "OrderBy expression is null");

            offset = offset > 1 ? offset : 0;

            var query = includes is null ? this.Entities : includes(this.Entities);

            query = query.Where(where).Skip(offset).Take(limit);

            return orderAsAscending
                ? await query.OrderBy(orderBy).ToListAsync(cancellationToken).ConfigureAwait(false)
                : await query.OrderByDescending(orderBy).ToListAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
