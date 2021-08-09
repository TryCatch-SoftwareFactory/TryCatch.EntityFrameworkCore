// <copyright file="RepositoryBase{TEntity,TDbContext}.cs" company="TryCatch Software Factory">
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
    /// Abstract implementation of Command Repository with Unit Of Work pattern. Allows working with NO TRACKING entities and
    /// apply the data merge in an internal way.
    /// </summary>
    /// <typeparam name="TEntity">Type of entity.</typeparam>
    /// <typeparam name="TDbContext">Type of DbContext.</typeparam>
    public abstract class RepositoryBase<TEntity, TDbContext> : ICommandRepository<TEntity>
        where TEntity : class
        where TDbContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryBase{TEntity, TDbContext}"/> class.
        /// </summary>
        /// <param name="context">Reference to DB context.</param>
        /// <param name="expressionsFactory">Reference to expressions factory.</param>
        /// <param name="merger">Reference to the Entity merger.</param>
        protected RepositoryBase(
            TDbContext context,
            IExpressionsFactory<TEntity> expressionsFactory,
            IEntityMerger<TEntity> merger)
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

            var entityState = await this.Entities
                .AddAsync(entity, cancellationToken)
                .ConfigureAwait(false);

            return entityState.State == EntityState.Added;
        }

        /// <inheritdoc />
        public async Task<bool> AddAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            ArgumentsValidator.ThrowIfIsNull(entities);

            var result = false;

            if (entities.Any())
            {
                var entityStatus = entities.Select(x => this.Context.Attach(x));

                result = !entityStatus.Any(x => x.State != EntityState.Added);

                await this.Entities
                    .AddRangeAsync(entities, cancellationToken)
                    .ConfigureAwait(false);
            }

            return result;
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

            var result = false;

            if (entityToDelete != default(TEntity))
            {
                var entityState = this.Entities.Remove(entityToDelete);

                result = entityState.State == EntityState.Deleted;
            }

            return result;
        }

        /// <inheritdoc />
        public async Task<bool> DeleteAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            ArgumentsValidator.ThrowIfIsNull(entities);

            var result = false;

            if (entities.Any())
            {
                var entityStatus = entities.Select(x => this.Context.Attach(x));

                this.Entities.RemoveRange(entities);

                result = !entityStatus.Any(x => x.State == EntityState.Deleted);
            }

            return await Task.FromResult(result).ConfigureAwait(false);
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

            var result = false;

            if (entityToUpdate != default(TEntity))
            {
                entityToUpdate = this.Merger.MergeOnUpdate(entityToUpdate, entity);

                var entityState = this.Entities.Update(entityToUpdate);

                result = entityState.State == EntityState.Modified;
            }

            return await Task.FromResult(result).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<bool> UpdateAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            ArgumentsValidator.ThrowIfIsNull(entities);

            var isAllDeleted = false;

            if (entities.Any())
            {
                var entityStatus = entities.Select(x => this.Context.Attach(x));

                this.Entities.UpdateRange(entities);

                isAllDeleted = !entityStatus.Any(x => x.State == EntityState.Modified);
            }

            return await Task.FromResult(isAllDeleted).ConfigureAwait(false);
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
    }
}
