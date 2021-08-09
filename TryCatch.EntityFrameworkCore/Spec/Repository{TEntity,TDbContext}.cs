// <copyright file="Repository{TEntity,TDbContext}.cs" company="TryCatch Software Factory">
// Copyright © TryCatch Software Factory All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>

namespace TryCatch.EntityFrameworkCore.Spec
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using TryCatch.Mergers;
    using TryCatch.Patterns.Repositories;
    using TryCatch.Patterns.Specifications;
    using TryCatch.Patterns.Specifications.Linq;
    using TryCatch.Validators;

    /// <summary>
    /// Abstract implementation of repository. Allows working with TRACKING entities and apply the data merge in an internal way.
    /// </summary>
    /// <typeparam name="TEntity">Type of entity.</typeparam>
    /// <typeparam name="TDbContext">Type of DbContext.</typeparam>
    public abstract class Repository<TEntity, TDbContext> : Linq.Repository<TEntity, TDbContext>, ISpecRepository<TEntity>
        where TEntity : class
        where TDbContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{TEntity, TDbContext}"/> class.
        /// </summary>
        /// <param name="context">Reference to DB context.</param>
        /// <param name="expressionsFactory">Reference to expressions factory.</param>
        /// <param name="merger">Reference to the Entity merger.</param>
        protected Repository(TDbContext context, IExpressionsFactory<TEntity> expressionsFactory, IEntityMerger<TEntity> merger)
            : base(context, expressionsFactory, merger)
        {
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteAsync(ISpecification<TEntity> spec, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var where = spec as ILinqSpecification<TEntity>;

            ArgumentsValidator.ThrowIfIsNull(where);

            var asTracking = true;
            var includes = this.ExpressionsFactory.GetIncludesByQuerName(QueriesNames.DeleteOne);
            var entityToDelete = await this.GetAsync(asTracking, where.AsExpression(), includes, cancellationToken).ConfigureAwait(false);

            if (entityToDelete != default(TEntity))
            {
                this.Entities.Remove(entityToDelete);
            }

            return await this.SaveChanges(cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<TEntity> GetAsync(ISpecification<TEntity> where, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var spec = where as ILinqSpecification<TEntity>;

            ArgumentsValidator.ThrowIfIsNull(spec);

            return await this.GetAsync(spec.AsExpression(), cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<long> GetCountAsync(ISpecification<TEntity> where = null, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var spec = where as ILinqSpecification<TEntity>;

            var expression = spec is null ? null : spec.AsExpression();

            return await this.GetCountAsync(expression, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<TEntity>> GetPageAsync(
            int offset = 1,
            int limit = 1000,
            ISpecification<TEntity> where = null,
            ISortSpecification<TEntity> orderBy = null,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            ArgumentsValidator.ThrowIfIsLessThan(1, offset, $"Offset value is invalid: {offset}");
            ArgumentsValidator.ThrowIfIsLessThan(1, limit, $"Limit value is invalid: {limit}");

            var specs = where as ILinqSpecification<TEntity>;

            var whereSpec = specs?.AsExpression();
            var orderBySpec = orderBy?.AsExpression();
            var orderAsAsc = orderBy is null || orderBy.IsAscending();

            return await this.GetPageAsync(offset, limit, whereSpec, orderBySpec, orderAsAsc, cancellationToken).ConfigureAwait(false);
        }
    }
}
