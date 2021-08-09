// <copyright file="Repository{TEntity,TDbContext}.cs" company="TryCatch Software Factory">
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
    using TryCatch.Mergers;
    using TryCatch.Patterns.Repositories;
    using TryCatch.Validators;

    /// <summary>
    /// Abstract implementation of repository. Allows working with TRACKING entities and apply the data merge in an internal way.
    /// </summary>
    /// <typeparam name="TEntity">Type of entity.</typeparam>
    /// <typeparam name="TDbContext">Type of DbContext.</typeparam>
    public abstract class Repository<TEntity, TDbContext> : ILinqRepository<TEntity>, IDisposable
        where TEntity : class
        where TDbContext : DbContext
    {
        private const int DefaultLimit = 1000;

        private bool _disposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{TEntity, TDbContext}"/> class.
        /// </summary>
        /// <param name="context">Reference to DB context.</param>
        /// <param name="expressionsFactory">Reference to expressions factory.</param>
        /// <param name="merger">Reference to the Entity merger.</param>
        protected Repository(TDbContext context, IExpressionsFactory<TEntity> expressionsFactory, IEntityMerger<TEntity> merger)
        {
            ArgumentsValidator.ThrowIfIsNull(context);
            ArgumentsValidator.ThrowIfIsNull(merger);
            ArgumentsValidator.ThrowIfIsNull(expressionsFactory);

            this.Context = context;
            this.Merger = merger;
            this.ExpressionsFactory = expressionsFactory;
            this.Entities = this.Context.Set<TEntity>();
        }

        protected TDbContext Context { get; }

        protected DbSet<TEntity> Entities { get; }

        protected IExpressionsFactory<TEntity> ExpressionsFactory { get; }

        protected IEntityMerger<TEntity> Merger { get; }

        /// <inheritdoc/>
        public async Task<bool> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            ArgumentsValidator.ThrowIfIsNull(entity);

            _ = await this.Entities
                .AddAsync(entity, cancellationToken)
                .ConfigureAwait(false);

            return await this.SaveChanges(cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<bool> AddAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            ArgumentsValidator.ThrowIfIsNull(entities);

            if (entities.Any())
            {
                await this.Entities
                    .AddRangeAsync(entities, cancellationToken)
                    .ConfigureAwait(false);
            }

            return await this.SaveChanges(cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<bool> AddOrUpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            ArgumentsValidator.ThrowIfIsNull(entity);

            var asTracking = false;

            var where = this.ExpressionsFactory.GetWhereByQueryName(QueriesNames.DefaultGet, entity);

            var entityToUpdate = await this.GetAsync(asTracking, where, includes: null, cancellationToken).ConfigureAwait(false);

            return entityToUpdate is default(TEntity)
                ? await this.AddAsync(entity, cancellationToken).ConfigureAwait(false)
                : await this.UpdateAsync(entity, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            ArgumentsValidator.ThrowIfIsNull(entity);

            var asTracking = true;
            var where = this.ExpressionsFactory.GetWhereByQueryName(QueriesNames.DeleteOne, entity);
            var includes = this.ExpressionsFactory.GetIncludesByQuerName(QueriesNames.DeleteOne);

            var entityToDelete = await this.GetAsync(asTracking, where, includes, cancellationToken).ConfigureAwait(false);

            if (entityToDelete != default(TEntity))
            {
                this.Entities.Remove(entityToDelete);
            }

            return await this.SaveChanges(cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            ArgumentsValidator.ThrowIfIsNull(entities);

            if (entities.Any())
            {
                this.Entities.RemoveRange(entities);
            }

            return await this.SaveChanges(cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            // Dispose of unmanaged resources.
            this.Dispose(true);

            // Suppress finalization.
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc/>
        public async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> where, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            ArgumentsValidator.ThrowIfIsNull(where);

            var asTracking = false;

            var includes = this.ExpressionsFactory.GetIncludesByQuerName(QueriesNames.DefaultGet);

            return await this.GetAsync(asTracking, where, includes, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<long> GetCountAsync(Expression<Func<TEntity, bool>> where = null, CancellationToken cancellationToken = default)
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
                .GetPageAsync(offset, limit, where, orderBy, includes, orderAsAscending,  cancellationToken)
                .ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<bool> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            ArgumentsValidator.ThrowIfIsNull(entity);

            var asTracking = true;
            var where = this.ExpressionsFactory.GetWhereByQueryName(QueriesNames.UpdateOne, entity);
            var includes = this.ExpressionsFactory.GetIncludesByQuerName(QueriesNames.UpdateOne);

            var entityToUpdate = await this.GetAsync(asTracking, where, includes, cancellationToken).ConfigureAwait(false);

            if (entityToUpdate != default(TEntity))
            {
                entityToUpdate = this.Merger.MergeOnUpdate(entityToUpdate, entity);

                this.Entities.Update(entityToUpdate);
            }

            return await this.SaveChanges(cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<bool> UpdateAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            ArgumentsValidator.ThrowIfIsNull(entities);

            if (entities.Any())
            {
                this.Entities.UpdateRange(entities);
            }

            return await this.SaveChanges(cancellationToken).ConfigureAwait(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this._disposed)
            {
                return;
            }

            if (disposing)
            {
                this.Context.Dispose();
            }

            this._disposed = true;
        }

        protected async virtual Task<TEntity> GetAsync(
            bool asTracking,
            Expression<Func<TEntity, bool>> where,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includes,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var entities = asTracking ? this.Entities : this.Entities.AsNoTracking();

            var query = includes is null ? entities : includes(entities);

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

            var query = includes is null
                ? this.Entities.AsNoTracking()
                : includes(this.Entities.AsNoTracking());

            query = query.Where(where).Skip(offset).Take(limit);

            return orderAsAscending
                ? await query.OrderBy(orderBy).ToListAsync(cancellationToken).ConfigureAwait(false)
                : await query.OrderByDescending(orderBy).ToListAsync(cancellationToken).ConfigureAwait(false);
        }

        protected async virtual Task<bool> SaveChanges(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var affectedRows = await this.Context
                .SaveChangesAsync(cancellationToken)
                .ConfigureAwait(false);

            return affectedRows > 0;
        }
    }
}
