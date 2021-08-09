// <copyright file="CommandRepository{TEntity,TDbContext}.cs" company="TryCatch Software Factory">
// Copyright © TryCatch Software Factory All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>

namespace TryCatch.EntityFrameworkCore
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
    /// Abstract implementation of command repository. Allows working with NO TRACKING entities and apply the data merge in an internal way.
    /// </summary>
    /// <typeparam name="TEntity">Type of entity.</typeparam>
    /// <typeparam name="TDbContext">Type of DbContext.</typeparam>
    public abstract class CommandRepository<TEntity, TDbContext> : ICommandRepository<TEntity>, IDisposable
        where TEntity : class
        where TDbContext : DbContext
    {
        private bool disposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandRepository{TEntity, TDbContext}"/> class.
        /// </summary>
        /// <param name="context">Reference to DB context.</param>
        /// <param name="expressionsFactory">Reference to expressions factory.</param>
        /// <param name="merger">Reference to the Entity merger.</param>
        protected CommandRepository(TDbContext context, IExpressionsFactory<TEntity> expressionsFactory, IEntityMerger<TEntity> merger)
        {
            ArgumentsValidator.ThrowIfIsNull(context);
            ArgumentsValidator.ThrowIfIsNull(expressionsFactory);
            ArgumentsValidator.ThrowIfIsNull(merger);

            this.Context = context;
            this.Merger = merger;
            this.ExpressionsFactory = expressionsFactory;
            this.Entities = this.Context.Set<TEntity>();
        }

        protected TDbContext Context { get; }

        protected DbSet<TEntity> Entities { get; }

        protected IExpressionsFactory<TEntity> ExpressionsFactory { get; }

        protected IEntityMerger<TEntity> Merger { get; }

        /// <inheritdoc />
        public async Task<bool> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            ArgumentsValidator.ThrowIfIsNull(entity);

            _ = await this.Entities
                .AddAsync(entity, cancellationToken)
                .ConfigureAwait(false);

            return await this.SaveChanges(cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<bool> AddAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            ArgumentsValidator.ThrowIfIsNull(entities);

            await this.Entities
                .AddRangeAsync(entities, cancellationToken)
                .ConfigureAwait(false);

            return await this.SaveChanges(cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
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

        /// <inheritdoc />
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

        /// <inheritdoc />
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

        /// <inheritdoc />
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
