// <copyright file="VehiclesCommandRepository.cs" company="TryCatch Software Factory">
// Copyright © TryCatch Software Factory All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>

namespace TryCatch.EntityFrameworkCore.UnitTests.Mocks
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore.Query;
    using TryCatch.EntityFrameworkCore.UnitTests.Mocks.Models;

    public class VehiclesCommandRepository : CommandRepository<Vehicle, VehiclesContext>
    {
        public VehiclesCommandRepository(
            VehiclesContext dbContext,
            VehiclesExpressionFactory factory,
            VehiclesMerger merger)
            : base(dbContext, factory, merger)
        {
        }

        public async Task<Vehicle> GetVehicleWithInclude(Func<IQueryable<Vehicle>, IIncludableQueryable<Vehicle, object>> includes, string name)
        {
            return await this.GetAsync(asTracking: false, where: (x) => x.Name.Contains(name), includes: includes).ConfigureAwait(false);
        }
    }
}
