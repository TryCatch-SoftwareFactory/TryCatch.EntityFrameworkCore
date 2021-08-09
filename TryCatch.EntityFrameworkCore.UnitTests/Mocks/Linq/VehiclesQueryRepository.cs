﻿// <copyright file="VehiclesQueryRepository.cs" company="TryCatch Software Factory">
// Copyright © TryCatch Software Factory All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>

namespace TryCatch.EntityFrameworkCore.UnitTests.Mocks.Linq
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore.Query;
    using TryCatch.EntityFrameworkCore.Linq;
    using TryCatch.EntityFrameworkCore.UnitTests.Mocks.Models;

    public class VehiclesQueryRepository : QueryRepository<Vehicle, VehiclesContext>
    {
        public VehiclesQueryRepository(VehiclesContext dbContext, VehiclesExpressionFactory factory)
            : base(dbContext, factory)
        {
        }

        public async Task<Vehicle> GetVehicleWithInclude(Func<IQueryable<Vehicle>, IIncludableQueryable<Vehicle, object>> includes, string name)
        {
            return await this.GetAsync(where: (x) => x.Name.Contains(name), includes: includes).ConfigureAwait(false);
        }

        public async Task<IEnumerable<Vehicle>> GetVehiclesWithInclude(Func<IQueryable<Vehicle>, IIncludableQueryable<Vehicle, object>> includes, string name)
        {
            var orderBy = this.ExpressionsFactory.GetSortByByQueryName(QueriesNames.DefaultPage);

            return await this.GetPageAsync(
                offset: 1, limit: 10, where: (x) => x.Name.Contains(name), orderBy: orderBy, includes: includes, orderAsAscending: true)
                .ConfigureAwait(false);
        }
    }
}
