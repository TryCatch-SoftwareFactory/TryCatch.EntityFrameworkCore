// <copyright file="DatabaseFixture.cs" company="TryCatch Software Factory">
// Copyright © TryCatch Software Factory All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>

namespace TryCatch.EntityFrameworkCore.UnitTests.Fixtures
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using TryCatch.EntityFrameworkCore.UnitTests.Mocks;
    using TryCatch.EntityFrameworkCore.UnitTests.Mocks.Models;

    public class DatabaseFixture : IDisposable
    {
        private static object dbLock = new object();

        private static bool databaseInitialized;

        private readonly DbContextOptionsBuilder<VehiclesContext> builder;

        private readonly VehiclesContext context;

        private bool disposed = false;

        public DatabaseFixture()
        {
            this.builder = new DbContextOptionsBuilder<VehiclesContext>()
                .UseInMemoryDatabase("test")
                .EnableDetailedErrors(true)
                .EnableSensitiveDataLogging(true);

            this.context = new VehiclesContext(this.builder.Options);

            lock (dbLock)
            {
                if (!databaseInitialized)
                {
                    this.context.Database.EnsureDeleted();
                    this.context.Database.EnsureCreated();
                    Given.SeedDatabase(this.context);
                }

                databaseInitialized = true;
            }
        }

        public VehiclesContext Context => new VehiclesContext(this.builder.Options);

        public void Dispose()
        {
            // Dispose of unmanaged resources.
            this.Dispose(true);

            // Suppress finalization.
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                this.context.Dispose();
            }

            this.disposed = true;
        }
    }
}
