// <copyright file="UnitOfWorkTests.cs" company="TryCatch Software Factory">
// Copyright © TryCatch Software Factory All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>

namespace TryCatch.EntityFrameworkCore.UnitTests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using NSubstitute;
    using TryCatch.EntityFrameworkCore.UnitTests.Fixtures;
    using TryCatch.EntityFrameworkCore.UnitTests.Mocks;
    using TryCatch.EntityFrameworkCore.UnitTests.Mocks.Models;
    using Xunit;

    public class UnitOfWorkTests : IClassFixture<DatabaseFixture>
    {
        private readonly DatabaseFixture fixture;

        public UnitOfWorkTests(DatabaseFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public void Construct_without_context()
        {
            // Arrange
            VehiclesContext context = null;

            // Act
            Action act = () => _ = new VehiclesUoW(context);

            // Asserts
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task Commit_Ok()
        {
            // Arrange
            var context = this.fixture.Context;
            var entity = EntitiesFactory.Get();

            using (var uow = new VehiclesUoW(context))
            {
                await context.AddAsync(entity).ConfigureAwait(false);

                // Act
                var actual = await uow.CommitAsync().ConfigureAwait(false);

                // Asserts
                actual.Should().BeTrue();

                context.Vehicles
                .FirstOrDefault(x => x.VehicleId == entity.VehicleId)
                .Should()
                .BeEquivalentTo(entity);
            }
        }

        [Fact]
        public async Task Commit_without_changes()
        {
            // Arrange
            var context = this.fixture.Context;

            using (var uow = new VehiclesUoW(context))
            {
                // Act
                var actual = await uow.CommitAsync().ConfigureAwait(false);

                // Asserts
                actual.Should().BeFalse();
            }
        }

        [Fact]
        public async Task Rollback_with_empty_changes()
        {
            // Arrange
            var context = this.fixture.Context;

            using (var uow = new VehiclesUoW(context))
            {
                // Act
                var actual = await uow.RollbackAsync().ConfigureAwait(false);

                // Asserts
                actual.Should().BeFalse();
            }
        }

        [Fact]
        public async Task Rollback_Ok()
        {
            // Arrange
            var context = this.fixture.Context;
            var entity = EntitiesFactory.Get();
            var entityToUpdate = EntitiesFactory.Get();
            var entityToDelete = EntitiesFactory.Get();
            await context.AddRangeAsync(new[] { entityToUpdate, entityToDelete }).ConfigureAwait(false);
            await context.SaveChangesAsync().ConfigureAwait(false);

            using (var uow = new VehiclesUoW(context))
            {
                entityToUpdate.Name = "entity-to-update-and-rollback";
                context.Update(entityToUpdate);
                context.Remove(entityToDelete);
                await context.AddAsync(entity).ConfigureAwait(false);

                // Act
                var actual = await uow.RollbackAsync().ConfigureAwait(false);
                var commit = await uow.CommitAsync().ConfigureAwait(false);

                // Asserts
                actual.Should().BeTrue();
                commit.Should().BeFalse();

                context.ChangeTracker.HasChanges().Should().BeFalse();
            }
        }

        [Fact]
        public void Called_only_one_time_to_context_dispose()
        {
            // Arrange
            var fakeDbContext = Substitute.For<VehiclesContext>();

            // Act
            using (var repository = new VehiclesUoW(fakeDbContext))
            {
                repository.Dispose();
                repository.Dispose();
                repository.Dispose();
            }

            // Asserts
            fakeDbContext.Received(1).Dispose();
        }
    }
}
