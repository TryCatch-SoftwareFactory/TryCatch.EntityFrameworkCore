// <copyright file="QueryRepository{TEntity,TDbContext}.cs" company="TryCatch Software Factory">
// Copyright © TryCatch Software Factory All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>

namespace TryCatch.EntityFrameworkCore.Spec
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using TryCatch.Patterns.Repositories;
    using TryCatch.Patterns.Specifications;
    using TryCatch.Patterns.Specifications.Linq;
    using TryCatch.Validators;

    /// <summary>
    /// Abstract implementation of Specification Query Repository. Allows working with NO TRACKING entities.
    /// </summary>
    /// <typeparam name="TEntity">Type of entity.</typeparam>
    /// <typeparam name="TDbContext">Type of DbContext.</typeparam>
    public abstract class QueryRepository<TEntity, TDbContext> : Linq.QueryRepository<TEntity, TDbContext>, ISpecQueryRepository<TEntity>
        where TEntity : class
        where TDbContext : DbContext
    {
        private const string SpecificationException = "Where is an invalid ILinqSpecification<TEntity>";

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryRepository{TEntity, TDbContext}"/> class.
        /// </summary>
        /// <param name="context">Reference to DbContext.</param>
        /// <param name="expressionsFactory">Reference to expressions factory.</param>
        protected QueryRepository(TDbContext context, IExpressionsFactory<TEntity> expressionsFactory)
            : base(context, expressionsFactory)
        {
        }

        /// <inheritdoc/>
        public async Task<TEntity> GetAsync(ISpecification<TEntity> where, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var spec = where as ILinqSpecification<TEntity>;

            ArgumentsValidator.ThrowIfIsNull(spec, SpecificationException);

            return await this
                .GetAsync(spec.AsExpression(), cancellationToken)
                .ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<long> GetCountAsync(ISpecification<TEntity> where = null, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var specs = where as ILinqSpecification<TEntity>;

            return await this.GetCountAsync(specs?.AsExpression(), cancellationToken).ConfigureAwait(false);
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
