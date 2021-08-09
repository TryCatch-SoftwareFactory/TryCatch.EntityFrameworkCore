// <copyright file="VehiclesQueryRepository.cs" company="TryCatch Software Factory">
// Copyright © TryCatch Software Factory All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>

namespace TryCatch.EntityFrameworkCore.UnitTests.Mocks.Spec
{
    using TryCatch.EntityFrameworkCore.Spec;
    using TryCatch.EntityFrameworkCore.UnitTests.Mocks.Models;

    public class VehiclesQueryRepository : QueryRepository<Vehicle, VehiclesContext>
    {
        public VehiclesQueryRepository(VehiclesContext dbContext, VehiclesExpressionFactory factory)
            : base(dbContext, factory)
        {
        }
    }
}
