// <copyright file="VehiclesContext.cs" company="TryCatch Software Factory">
// Copyright © TryCatch Software Factory All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>

namespace TryCatch.EntityFrameworkCore.UnitTests.Mocks.Models
{
    using Microsoft.EntityFrameworkCore;

    public class VehiclesContext : DbContext
    {
        public VehiclesContext()
        {
        }

        public VehiclesContext(DbContextOptions<VehiclesContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Vehicle> Vehicles { get; set; }

        public virtual DbSet<Wheel> Wheels { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Vehicle>(e =>
            {
                e.HasKey(x => x.VehicleId);
            });

            modelBuilder.Entity<Wheel>(e =>
            {
                e.HasKey(x => x.WheelId);
            });
        }
    }
}
