// <copyright file="UnitOfWork.cs" company="TryCatch Software Factory">
// Copyright © TryCatch Software Factory All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>

namespace TryCatch.EntityFrameworkCore
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using TryCatch.Patterns;
    using TryCatch.Validators;

    public abstract class UnitOfWork : IUnitOfWork
    {
        private readonly DbContext dbContext;

        private bool _disposed = false;

        protected UnitOfWork(DbContext dbContext)
        {
            ArgumentsValidator.ThrowIfIsNull(dbContext);

            this.dbContext = dbContext;
        }

        /// <inheritdoc />
        public async Task<bool> CommitAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var saved = await this.dbContext
                .SaveChangesAsync(cancellationToken)
                .ConfigureAwait(false);

            return saved > 0;
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
        public Task<bool> RollbackAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var hasChanges = this.dbContext.ChangeTracker.HasChanges();

            if (hasChanges)
            {
                foreach (var entry in this.dbContext.ChangeTracker.Entries())
                {
                    if (entry.State == EntityState.Modified || entry.State == EntityState.Deleted)
                    {
                        entry.State = EntityState.Unchanged;
                    }
                    else if (entry.State == EntityState.Added)
                    {
                        entry.State = EntityState.Detached;
                    }
                }
            }

            return Task.FromResult(hasChanges);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this._disposed)
            {
                return;
            }

            if (disposing)
            {
                this.dbContext.Dispose();
            }

            this._disposed = true;
        }
    }
}
