// <copyright file="VehiclesRepositoryBaseTests.cs" company="TryCatch Software Factory">
// Copyright © TryCatch Software Factory All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>

namespace TryCatch.EntityFrameworkCore.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Query;
    using TryCatch.EntityFrameworkCore.UnitTests.Fixtures;
    using TryCatch.EntityFrameworkCore.UnitTests.Mocks;
    using TryCatch.EntityFrameworkCore.UnitTests.Mocks.Models;
    using Xunit;

    public class VehiclesRepositoryBaseTests : IClassFixture<DatabaseFixture>, IDisposable
    {
        private const string TestName = "REPOSITORY-BASE-TEST";

        private readonly VehiclesRepositoryBase sut;

        private bool disposed = false;

        public VehiclesRepositoryBaseTests(DatabaseFixture databaseFixture)
        {
            var factory = new VehiclesExpressionFactory();

            var merger = new VehiclesMerger(databaseFixture.Context);

            this.sut = new VehiclesRepositoryBase(databaseFixture.Context, factory, merger);
        }

        [Fact]
        public async Task Add_without_entity()
        {
            // Arrange
            Vehicle entity = null;

            // Act
            Func<Task> act = async () => await this.sut.AddAsync(entity).ConfigureAwait(false);

            // Asserts
            await act.Should().ThrowAsync<ArgumentNullException>().ConfigureAwait(false);
        }

        [Fact]
        public async Task Add_ok()
        {
            // Arrange
            var entity = EntitiesFactory.Get();

            // Act
            var actual = await this.sut.AddAsync(entity).ConfigureAwait(false);

            // Asserts
            actual.Should().BeTrue();
        }

        [Fact]
        public async Task AddOrUpdate_without_entity()
        {
            // Arrange
            Vehicle entity = null;

            // Act
            Func<Task> act = async () => await this.sut.AddOrUpdateAsync(entity).ConfigureAwait(false);

            // Asserts
            await act.Should().ThrowAsync<ArgumentNullException>().ConfigureAwait(false);
        }

        [Fact]
        public async Task AddOrUpdate_new_entity_ok()
        {
            // Arrange
            var entity = EntitiesFactory.Get();

            // Act
            var actual = await this.sut.AddOrUpdateAsync(entity).ConfigureAwait(false);

            // Asserts
            actual.Should().BeTrue();
        }

        [Fact]
        public async Task AddOrUpdate_old_entity_ok()
        {
            // Arrange
            var entity = Given.OldVehicleToUpdate;

            entity.Name = $"{entity.Name}-MODIFIED-{TestName}-2";

            // Act
            var actual = await this.sut.AddOrUpdateAsync(entity).ConfigureAwait(false);

            // Asserts
            actual.Should().BeTrue();
        }

        [Fact]
        public async Task Add_without_entities()
        {
            // Arrange
            IEnumerable<Vehicle> entities = null;

            // Act
            Func<Task> act = async () => await this.sut.AddAsync(entities).ConfigureAwait(false);

            // Asserts
            await act.Should().ThrowAsync<ArgumentNullException>().ConfigureAwait(false);
        }

        [Fact]
        public async Task Add_with_empty_entities()
        {
            // Arrange
            var entities = Array.Empty<Vehicle>();

            // Act
            var actual = await this.sut.AddAsync(entities).ConfigureAwait(false);

            // Asserts
            actual.Should().BeFalse();
        }

        [Fact]
        public async Task Add_entities_ok()
        {
            // Arrange
            var entities = EntitiesFactory.Get(10);

            // Act
            var actual = await this.sut.AddAsync(entities).ConfigureAwait(false);

            // Asserts
            actual.Should().BeTrue();
        }

        [Fact]
        public async Task Update_without_entity()
        {
            // Arrange
            Vehicle entity = null;

            // Act
            Func<Task> act = async () => await this.sut.UpdateAsync(entity).ConfigureAwait(false);

            // Asserts
            await act.Should().ThrowAsync<ArgumentNullException>().ConfigureAwait(false);
        }

        [Fact]
        public async Task Update_with_non_exist_entity()
        {
            // Arrange
            var entity = EntitiesFactory.Get();

            // Act
            var actual = await this.sut.UpdateAsync(entity).ConfigureAwait(false);

            // Asserts
            actual.Should().BeFalse();
        }

        [Fact]
        public async Task Update_ok()
        {
            // Arrange
            var entity = new Vehicle
            {
                VehicleId = Given.OtherVehicleToUpdate.VehicleId,
            };

            entity.Name = $"{entity.Name}-MODIFIED-{TestName}";

            // Act
            var actual = await this.sut.UpdateAsync(entity).ConfigureAwait(false);

            // Asserts
            actual.Should().BeTrue();
        }

        [Fact]
        public async Task Update_without_entities()
        {
            // Arrange
            IEnumerable<Vehicle> entities = null;

            // Act
            Func<Task> act = async () => await this.sut.UpdateAsync(entities).ConfigureAwait(false);

            // Asserts
            await act.Should().ThrowAsync<ArgumentNullException>().ConfigureAwait(false);
        }

        [Fact]
        public async Task Update_with_empty_entities()
        {
            // Arrange
            var entities = Array.Empty<Vehicle>() as IEnumerable<Vehicle>;

            // Act
            var actual = await this.sut.UpdateAsync(entities).ConfigureAwait(false);

            // Asserts
            actual.Should().BeFalse();
        }

        [Fact]
        public async Task Update_with_non_exists_entities()
        {
            // Arrange
            var entities = EntitiesFactory.Get(10);

            // Act
            var actual = await this.sut.UpdateAsync(entities).ConfigureAwait(false);

            // Asserts
            actual.Should().BeTrue();
        }

        [Fact]
        public async Task Update_entities_ok()
        {
            // Arrange
            var entities = Given.OtherVehiclesToUpdate;

            entities = entities.Select(x => new Vehicle() { VehicleId = x.VehicleId, Name = $"{x.Name}-modified-{TestName}" }).ToList();

            // Act
            var actual = await this.sut.UpdateAsync(entities).ConfigureAwait(false);

            // Asserts
            actual.Should().BeTrue();
        }

        [Fact]
        public async Task Delete_without_entity()
        {
            // Arrange
            Vehicle entity = null;

            // Act
            Func<Task> act = async () => await this.sut.DeleteAsync(entity).ConfigureAwait(false);

            // Asserts
            await act.Should().ThrowAsync<ArgumentNullException>().ConfigureAwait(false);
        }

        [Fact]
        public async Task Delete_entity_ok()
        {
            // Arrange
            var entity = Given.VehicleToDeleteInLinqCommandTest2;

            // Act
            var actual = await this.sut.DeleteAsync(entity).ConfigureAwait(false);

            // Asserts
            actual.Should().BeTrue();
        }

        [Fact]
        public async Task Delete_entity_NotFound_ok()
        {
            // Arrange
            var entity = Given.NotFoundVehicle;

            // Act
            var actual = await this.sut.DeleteAsync(entity).ConfigureAwait(false);

            // Asserts
            actual.Should().BeFalse();
        }

        [Fact]
        public async Task Delete_without_entities()
        {
            // Arrange
            IEnumerable<Vehicle> entities = null;

            // Act
            Func<Task> act = async () => await this.sut.AddAsync(entities).ConfigureAwait(false);

            // Asserts
            await act.Should().ThrowAsync<ArgumentNullException>().ConfigureAwait(false);
        }

        [Fact]
        public async Task Delete_with_empty_entities()
        {
            // Arrange
            var entities = Array.Empty<Vehicle>();

            // Act
            var actual = await this.sut.DeleteAsync(entities).ConfigureAwait(false);

            // Asserts
            actual.Should().BeFalse();
        }

        [Fact]
        public async Task Delete_entities_ok()
        {
            // Arrange
            var entities = Given.VehiclesToDeleteLinqCommandTest;

            // Act
            var actual = await this.sut.DeleteAsync(entities).ConfigureAwait(false);

            // Asserts
            actual.Should().BeTrue();
        }

        [Fact]
        public async Task Internal_GetAsync_with_includes()
        {
            // Arrange
            Func<IQueryable<Vehicle>, IIncludableQueryable<Vehicle, object>> includes = (x) => x.Include(z => z.Wheels);
            var expected = Given.VehiclesWithWheels.First(x => x.Name == "vehicle-with-wheels-1");

            // Act
            var actual = await this.sut.GetVehicleWithInclude(includes: includes, "vehicle-with-wheels-1").ConfigureAwait(false);

            // Asserts
            actual.Should().BeEquivalentTo(expected, opt => opt.Excluding(y => y.CreatedAt));
        }

        [Fact]
        public async Task Internal_GetAsync_without_includes()
        {
            // Arrange
            Func<IQueryable<Vehicle>, IIncludableQueryable<Vehicle, object>> includes = null;
            var expected = Given.VehiclesWithWheels.First(x => x.Name == "vehicle-with-wheels-1");

            // Act
            var actual = await this.sut.GetVehicleWithInclude(includes: includes, "vehicle-with-wheels-1").ConfigureAwait(false);

            // Asserts
            actual.Should().BeEquivalentTo(expected, opt => opt.Excluding(y => y.CreatedAt).Excluding(y => y.Wheels));
        }

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
                // this.sut.Dispose();
            }

            this.disposed = true;
        }
    }
}
