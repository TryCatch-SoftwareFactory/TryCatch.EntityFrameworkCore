// <copyright file="RepositoryTests.cs" company="TryCatch Software Factory">
// Copyright © TryCatch Software Factory All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>

namespace TryCatch.EntityFrameworkCore.UnitTests.Spec
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using TryCatch.EntityFrameworkCore.UnitTests.Fixtures;
    using TryCatch.EntityFrameworkCore.UnitTests.Mocks;
    using TryCatch.EntityFrameworkCore.UnitTests.Mocks.Models;
    using TryCatch.EntityFrameworkCore.UnitTests.Mocks.Spec;
    using TryCatch.Patterns.Repositories;
    using TryCatch.Patterns.Specifications;
    using Xunit;

    public class RepositoryTests : IClassFixture<DatabaseFixture>, IDisposable
    {
        private const string TestName = "SPEC-REPOSITORY-TEST";

        private readonly ISpecRepository<Vehicle> sut;

        private bool disposed = false;

        public RepositoryTests(DatabaseFixture databaseFixture)
        {
            var factory = new VehiclesExpressionFactory();

            var merger = new VehiclesMerger(databaseFixture.Context);

            this.sut = new VehiclesRepository(databaseFixture.Context, factory, merger);
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
            var entity = Given.VehicleToUpdate;

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
            var entity = Given.VehicleToUpdate;

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
            var entities = Given.VehiclesToUpdate;

            entities = entities.Select(x => new Vehicle() { VehicleId = x.VehicleId, Name = $"{x.Name}-modified-{TestName}" });

            // Act
            var actual = await this.sut.UpdateAsync(entities).ConfigureAwait(false);

            // Asserts
            actual.Should().BeTrue();
        }

        [Fact]
        public async Task Delete_without_entity()
        {
            // Arrange
            ISpecification<Vehicle> spec = null;

            // Act
            Func<Task> act = async () => await this.sut.DeleteAsync(spec).ConfigureAwait(false);

            // Asserts
            await act.Should().ThrowAsync<ArgumentNullException>().ConfigureAwait(false);
        }

        [Fact]
        public async Task Delete_entity_ok()
        {
            // Arrange
            var spec = Given.VehicleToDeleteSpecRepositoryTest;

            // Act
            var actual = await this.sut.DeleteAsync(spec).ConfigureAwait(false);

            // Asserts
            actual.Should().BeTrue();
        }

        [Fact]
        public async Task Delete_not_found_entity_Ok()
        {
            // Arrange
            var spec = Given.NotFoundVehicleSpec;

            // Act
            var actual = await this.sut.DeleteAsync(spec).ConfigureAwait(false);

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
            var entities = Given.VehiclesToDeleteSpecRepositoryTest;

            // Act
            var actual = await this.sut.DeleteAsync(entities).ConfigureAwait(false);

            // Asserts
            actual.Should().BeTrue();
        }

        [Fact]
        public async Task GetAsync_without_where()
        {
            // Arrange
            ISpecification<Vehicle> spec = null;

            // Act
            Func<Task> act = async () => await this.sut.GetAsync(spec).ConfigureAwait(false);

            // Asserts
            await act.Should().ThrowAsync<ArgumentNullException>().ConfigureAwait(false);
        }

        [Fact]
        public async Task GetAsync_ok()
        {
            // Arrange
            var expectedEntity = Given.GetVehicle;

            // Act
            var actual = await this.sut.GetAsync(Given.SpecReadWhere).ConfigureAwait(false);

            // Asserts
            actual.Should().BeEquivalentTo(expectedEntity);
        }

        [Fact]
        public async Task GetAsync_not_found_ok()
        {
            // Arrange
            var expectedEntity = default(Vehicle);

            // Act
            var actual = await this.sut.GetAsync(Given.NotFoundVehicleSpec).ConfigureAwait(false);

            // Asserts
            actual.Should().BeEquivalentTo(expectedEntity);
        }

        [Fact]
        public async Task GetCountAsync_without_where()
        {
            // Arrange
            ISpecification<Vehicle> where = null;

            var expectedLength = Given.GetVehicles.Count();

            // Act
            var actual = await this.sut.GetCountAsync(where).ConfigureAwait(false);

            // Asserts
            actual.Should().Be(expectedLength);
        }

        [Fact]
        public async Task GetCountAsync_with_where()
        {
            // Arrange
            var expected = 1;

            // Act
            var actual = await this.sut.GetCountAsync(Given.SpecReadWhere).ConfigureAwait(false);

            // Asserts
            actual.Should().Be(expected);
        }

        [Theory]
        [InlineData(-1, 1)]
        [InlineData(1, -1)]
        public async Task GetPageAsync_with_invalid_args(int offset, int limit)
        {
            // Arrange

            // Act
            Func<Task> act = async () => await this.sut
                .GetPageAsync(offset, limit)
                .ConfigureAwait(false);

            // Asserts
            await act.Should()
                .ThrowAsync<ArgumentOutOfRangeException>()
                .ConfigureAwait(false);
        }

        [Theory]
        [InlineData(2, 2, 2)]
        [InlineData(1, 1, 1)]
        public async Task GetPageAsync_with_valid_args(int offset, int limit, int expectedLength)
        {
            // Arrange

            // Act
            var actual = await this.sut
                .GetPageAsync(limit: limit, offset: offset)
                .ConfigureAwait(false);

            // Asserts
            actual.Should().HaveCount(expectedLength);
        }

        [Fact]
        public async Task GetPageAsync_with_where()
        {
            // Arrange
            var where = Given.SpecListWhere;
            var expected = Given.GetFilteredVehicles();

            // Act
            var actual = await this.sut
                .GetPageAsync(where: where)
                .ConfigureAwait(false);

            // Asserts
            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task GetPageAsync_with_default_where()
        {
            // Arrange
            var expected = Given.GetVehicles;

            // Act
            var actual = await this.sut
                .GetPageAsync()
                .ConfigureAwait(false);

            // Asserts
            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task GetPageAsync_orderBy_field_asc()
        {
            // Arrange
            var orderBy = Given.SpecOrderBy;
            var expected = Given.GetVehicles;

            // Act
            var actual = await this.sut
                .GetPageAsync(orderBy: orderBy)
                .ConfigureAwait(false);

            // Asserts
            actual.Should().BeEquivalentTo(expected).And.BeInAscendingOrder(x => x.Name);
        }

        [Fact]
        public async Task GetPageAsync_orderBy_field_desc()
        {
            // Arrange
            var orderBy = Given.SpecOrderByDesc;
            var expected = Given.GetVehicles;

            // Act
            var actual = await this.sut
                .GetPageAsync(orderBy: orderBy)
                .ConfigureAwait(false);

            // Asserts
            actual.Should().BeEquivalentTo(expected).And.BeInDescendingOrder(x => x.Name);
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
                (this.sut as VehiclesRepository).Dispose();
            }

            this.disposed = true;
        }
    }
}
