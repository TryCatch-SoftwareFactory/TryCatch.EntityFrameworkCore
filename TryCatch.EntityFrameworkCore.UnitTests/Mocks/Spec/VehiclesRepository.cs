// <copyright file="VehiclesRepository.cs" company="TryCatch Software Factory">
// Copyright © TryCatch Software Factory All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>

namespace TryCatch.EntityFrameworkCore.UnitTests.Mocks.Spec
{
    using TryCatch.EntityFrameworkCore.Spec;
    using TryCatch.EntityFrameworkCore.UnitTests.Mocks.Models;

    public class VehiclesRepository : Repository<Vehicle, VehiclesContext>
    {
        public VehiclesRepository(
            VehiclesContext dbContext,
            VehiclesExpressionFactory factory,
            VehiclesMerger merger)
            : base(dbContext, factory, merger)
        {
        }
    }
}
