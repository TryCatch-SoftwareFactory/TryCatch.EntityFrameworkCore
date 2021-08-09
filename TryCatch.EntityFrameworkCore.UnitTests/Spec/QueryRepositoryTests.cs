// <copyright file="QueryRepositoryTests.cs" company="TryCatch Software Factory">
// Copyright © TryCatch Software Factory All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>

namespace TryCatch.EntityFrameworkCore.UnitTests.Spec
{
    using System;
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

    public class QueryRepositoryTests : IClassFixture<DatabaseFixture>, IDisposable
    {
        private readonly ISpecQueryRepository<Vehicle> sut;

        private bool disposed = false;

        public QueryRepositoryTests(DatabaseFixture databaseFixture)
        {
            var factory = new VehiclesExpressionFactory();

            this.sut = new VehiclesQueryRepository(databaseFixture.Context, factory);
        }

        [Fact]
        public async Task GetAsync_without_where()
        {
            // Arrange
            ISpecification<Vehicle> where = null;

            // Act
            Func<Task> act = async () => await this.sut.GetAsync(where).ConfigureAwait(false);

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
                (this.sut as VehiclesQueryRepository).Dispose();
            }

            this.disposed = true;
        }
    }
}
