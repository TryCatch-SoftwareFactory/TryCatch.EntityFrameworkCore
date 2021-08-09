// <copyright file="VehiclesMerger.cs" company="TryCatch Software Factory">
// Copyright © TryCatch Software Factory All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>

namespace TryCatch.EntityFrameworkCore.UnitTests.Mocks
{
    using System;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;
    using TryCatch.EntityFrameworkCore.UnitTests.Mocks.Models;
    using TryCatch.Mergers;

    public class VehiclesMerger : IEntityMerger<Vehicle>
    {
        private readonly VehiclesContext context;

        public VehiclesMerger(VehiclesContext context)
        {
            this.context = context;
        }

        public Vehicle MergeOnUpdate(Vehicle currentEntity, Vehicle newEntity)
        {
            currentEntity.Name = newEntity.Name;
            currentEntity.UpdatedAt = DateTime.UtcNow;

            _ = currentEntity.Wheels
                .Where(x => !newEntity.Wheels.Any(y => y.WheelId == x.WheelId))
                .Select(x =>
                {
                    this.context.Entry(x).State = EntityState.Deleted;

                    return x;
                });

            _ = currentEntity.Wheels
                .Where(x => newEntity.Wheels.Any(y => y.WheelId == x.WheelId && y.WheelType != x.WheelType))
                .Select(x =>
                {
                    this.context.Entry(x).State = EntityState.Modified;
                    return x;
                });

            _ = newEntity.Wheels
                .Where(x => !currentEntity.Wheels.Any(y => y.WheelId == x.WheelId))
                .Select(x =>
                {
                    this.context.Entry(x).State = EntityState.Added;

                    return x;
                });

            return currentEntity;
        }
    }
}
