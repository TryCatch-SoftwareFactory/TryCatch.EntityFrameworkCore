// <copyright file="VehiclesExpressionFactory.cs" company="TryCatch Software Factory">
// Copyright © TryCatch Software Factory All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>

namespace TryCatch.EntityFrameworkCore.UnitTests.Mocks
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Query;
    using TryCatch.EntityFrameworkCore.UnitTests.Mocks.Models;

    public class VehiclesExpressionFactory : IExpressionsFactory<Vehicle>
    {
        public Func<IQueryable<Vehicle>, IIncludableQueryable<Vehicle, object>> GetIncludesByQuerName(string queryName) =>
            queryName switch
            {
                QueriesNames.DefaultGet => (x) => x.Include(y => y.Wheels),
                _ => null
            };

        public Expression<Func<Vehicle, object>> GetSortByByQueryName(string queryName) =>
            queryName switch
            {
                _ => (x) => x.Name,
            };

        public Expression<Func<Vehicle, bool>> GetWhereByQueryName(string queryName, Vehicle entity = null) =>
            queryName switch
            {
                QueriesNames.DefaultGet => (x) => x.VehicleId == entity.VehicleId,
                QueriesNames.UpdateOne => (x) => x.VehicleId == entity.VehicleId,
                QueriesNames.DeleteOne => (x) => x.VehicleId == entity.VehicleId,
                _ => (x) => x.Name.Contains("read-Name")
            };
    }
}
