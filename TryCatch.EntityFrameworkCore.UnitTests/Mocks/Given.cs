// <copyright file="Given.cs" company="TryCatch Software Factory">
// Copyright © TryCatch Software Factory All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>

namespace TryCatch.EntityFrameworkCore.UnitTests.Mocks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using TryCatch.EntityFrameworkCore.UnitTests.Mocks.Models;
    using TryCatch.EntityFrameworkCore.UnitTests.Mocks.Spec;
    using TryCatch.Patterns.Specifications;

    public static class Given
    {
        private const string DefaultName = "read-Name-0";

        public static IEnumerable<Vehicle> GetVehicles => new[]
        {
            new Vehicle() { VehicleId = 99991, Name = "read-Name-0" },
            new Vehicle() { VehicleId = 99992, Name = "read-Name-1" },
            new Vehicle() { VehicleId = 99993, Name = "read-Name-2" },
            new Vehicle() { VehicleId = 99994, Name = "read-Name-3" },
            new Vehicle() { VehicleId = 99995, Name = "read-Name-4" },
            new Vehicle() { VehicleId = 99996, Name = "read-Name-5" },
            new Vehicle() { VehicleId = 99997, Name = "read-Name-6" },
            new Vehicle() { VehicleId = 99998, Name = "read-Name-7" },
            new Vehicle() { VehicleId = 99999, Name = "read-Name-8" },
            new Vehicle() { VehicleId = 999910, Name = "read-Name-9" },
            new Vehicle() { VehicleId = 999911, Name = "read-Name-01" },
            new Vehicle() { VehicleId = 999912, Name = "read-Name-02" },
            new Vehicle() { VehicleId = 999913, Name = "read-Name-03" },
            new Vehicle() { VehicleId = 999914, Name = "read-Name-04" },
            new Vehicle() { VehicleId = 999915, Name = "read-Name-05" },
            new Vehicle() { VehicleId = 999916, Name = "read-Name-06" },
            new Vehicle() { VehicleId = 999917, Name = "read-Name-07" },
            new Vehicle() { VehicleId = 999918, Name = "read-Name-08" },
            new Vehicle() { VehicleId = 999919, Name = "read-Name-09" },
            new Vehicle() { VehicleId = 999920, Name = "read-Name-010" },
            new Vehicle() { VehicleId = 999921, Name = "read-Name-011" },
            new Vehicle() { VehicleId = 999922, Name = "read-Name-012" },
            new Vehicle() { VehicleId = 999923, Name = "read-Name-013" },
            new Vehicle() { VehicleId = 999924, Name = "read-Name-014" },
            new Vehicle() { VehicleId = 999925, Name = "read-Name-015" },
            new Vehicle() { VehicleId = 999926, Name = "read-Name-016" },
            new Vehicle() { VehicleId = 999927, Name = "read-Name-017" },
            new Vehicle() { VehicleId = 999928, Name = "read-Name-018" },
            new Vehicle() { VehicleId = 999929, Name = "read-Name-019" },
        };

        public static Vehicle GetVehicle => GetVehicles.AsQueryable().First(ReadWhere);

        public static Expression<Func<Vehicle, bool>> NotFoundWhere => (x) => x.VehicleId == NotFoundVehicle.VehicleId;

        public static Expression<Func<Vehicle, bool>> ReadWhere => (x) => x.Name == DefaultName;

        public static Expression<Func<Vehicle, bool>> ListWhere => (x) => x.Name.Contains("read-Name");

        public static Expression<Func<Vehicle, object>> OrderBy => (x) => x.Name;

        public static ISpecification<Vehicle> SpecReadWhere => new ReadVehicleSpec(DefaultName);

        public static ISpecification<Vehicle> SpecListWhere => new ListVehiclesSpec("read-Name");

        public static ISortSpecification<Vehicle> SpecOrderBy => new SortVehicleSpec(true, "Name");

        public static ISortSpecification<Vehicle> SpecOrderByDesc => new SortVehicleSpec(false, "Name");

        public static Vehicle VehicleToDeleteLinqCommandTest => new Vehicle
        {
            VehicleId = 99991000,
            Name = "vehicle-2-delete",
        };

        public static Vehicle VehicleToDeleteInLinqCommandTest2 => new Vehicle
        {
            VehicleId = 98991000,
            Name = "vehicle-2-delete",
        };

        public static Vehicle VehicleToDeleteLinqRepositoryTest => new Vehicle
        {
            VehicleId = 99991001,
            Name = "vehicle-2-delete",
        };

        public static Vehicle NotFoundVehicle => new Vehicle
        {
            VehicleId = 999999999,
            Name = "not-found-vehicle-to-delete",
        };

        public static Vehicle ToDeleteWithSpecRepository => new Vehicle
        {
            VehicleId = 1000096,
            Name = "to-delete-with-spec-repository",
        };

        public static ISpecification<Vehicle> VehicleToDeleteSpecRepositoryTest => new DeleteVehicleSpec(ToDeleteWithSpecRepository.VehicleId);

        public static ISpecification<Vehicle> NotFoundVehicleSpec => new DeleteVehicleSpec(0);

        public static Vehicle VehicleToUpdate => new Vehicle
        {
            VehicleId = 99992000,
            Name = "vehicle-2-update",
        };

        public static Vehicle OtherVehicleToUpdate => new Vehicle
        {
            VehicleId = 99912000,
            Name = "vehicle-3-update",
        };

        public static Vehicle OldVehicleToUpdate => new Vehicle
        {
            VehicleId = 99992001,
            Name = "vehicle-old-update",
        };

        public static IEnumerable<Vehicle> VehiclesWithWheels => new HashSet<Vehicle>()
        {
            new Vehicle
            {
                Name = "vehicle-with-wheels-1",
                VehicleId = 101010,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null,
                Wheels = new List<Wheel>()
                {
                    new Wheel() { WheelType = "type-1", WheelId = 100120 },
                    new Wheel() { WheelType = "type-2", WheelId = 100121 },
                    new Wheel() { WheelType = "type-3", WheelId = 100122 },
                    new Wheel() { WheelType = "type-4", WheelId = 100123 },
                },
            },
            new Vehicle
            {
                Name = "vehicle-with-wheels-2",
                VehicleId = 202020,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null,
                Wheels = new List<Wheel>()
                {
                    new Wheel() { WheelType = "type-1", WheelId = 100124 },
                    new Wheel() { WheelType = "type-2", WheelId = 100125 },
                    new Wheel() { WheelType = "type-3", WheelId = 100126 },
                    new Wheel() { WheelType = "type-4", WheelId = 100127 },
                },
            },
        };

        public static IEnumerable<Vehicle> VehiclesToDeleteLinqCommandTest => new[]
        {
            new Vehicle() { VehicleId = 9999101, Name = "Vehicles-to-delete" },
            new Vehicle() { VehicleId = 9999102, Name = "Vehicles-to-delete" },
            new Vehicle() { VehicleId = 9999103, Name = "Vehicles-to-delete" },
            new Vehicle() { VehicleId = 9999104, Name = "Vehicles-to-delete" },
            new Vehicle() { VehicleId = 9999105, Name = "Vehicles-to-delete" },
            new Vehicle() { VehicleId = 9999106, Name = "Vehicles-to-delete" },
            new Vehicle() { VehicleId = 9999107, Name = "Vehicles-to-delete" },
        };

        public static IEnumerable<Vehicle> VehiclesToDeleteLinqRepositoryTest => new[]
        {
            new Vehicle() { VehicleId = 9999201, Name = "Vehicles-to-delete" },
            new Vehicle() { VehicleId = 9999202, Name = "Vehicles-to-delete" },
            new Vehicle() { VehicleId = 9999203, Name = "Vehicles-to-delete" },
            new Vehicle() { VehicleId = 9999204, Name = "Vehicles-to-delete" },
            new Vehicle() { VehicleId = 9999205, Name = "Vehicles-to-delete" },
            new Vehicle() { VehicleId = 9999206, Name = "Vehicles-to-delete" },
            new Vehicle() { VehicleId = 9999207, Name = "Vehicles-to-delete" },
        };

        public static IEnumerable<Vehicle> VehiclesToDeleteSpecCommandTest => new[]
        {
            new Vehicle() { VehicleId = 9999301, Name = "Vehicles-to-delete" },
            new Vehicle() { VehicleId = 9999302, Name = "Vehicles-to-delete" },
            new Vehicle() { VehicleId = 9999303, Name = "Vehicles-to-delete" },
            new Vehicle() { VehicleId = 9999304, Name = "Vehicles-to-delete" },
            new Vehicle() { VehicleId = 9999305, Name = "Vehicles-to-delete" },
            new Vehicle() { VehicleId = 9999306, Name = "Vehicles-to-delete" },
            new Vehicle() { VehicleId = 9999307, Name = "Vehicles-to-delete" },
        };

        public static IEnumerable<Vehicle> VehiclesToDeleteSpecRepositoryTest => new[]
        {
            new Vehicle() { VehicleId = 9999401, Name = "Vehicles-to-delete" },
            new Vehicle() { VehicleId = 9999402, Name = "Vehicles-to-delete" },
            new Vehicle() { VehicleId = 9999403, Name = "Vehicles-to-delete" },
            new Vehicle() { VehicleId = 9999404, Name = "Vehicles-to-delete" },
            new Vehicle() { VehicleId = 9999405, Name = "Vehicles-to-delete" },
            new Vehicle() { VehicleId = 9999406, Name = "Vehicles-to-delete" },
            new Vehicle() { VehicleId = 9999407, Name = "Vehicles-to-delete" },
        };

        public static IEnumerable<Vehicle> VehiclesToUpdate => new[]
        {
            new Vehicle() { VehicleId = 9999501, Name = "Vehicles-to-update" },
            new Vehicle() { VehicleId = 9999502, Name = "Vehicles-to-update" },
            new Vehicle() { VehicleId = 9999503, Name = "Vehicles-to-update" },
            new Vehicle() { VehicleId = 9999504, Name = "Vehicles-to-update" },
            new Vehicle() { VehicleId = 9999505, Name = "Vehicles-to-update" },
            new Vehicle() { VehicleId = 9999506, Name = "Vehicles-to-update" },
        };

        public static IEnumerable<Vehicle> OtherVehiclesToUpdate => new[]
        {
            new Vehicle() { VehicleId = 19999501, Name = "Vehicles-to-update" },
            new Vehicle() { VehicleId = 19999502, Name = "Vehicles-to-update" },
            new Vehicle() { VehicleId = 19999503, Name = "Vehicles-to-update" },
            new Vehicle() { VehicleId = 19999504, Name = "Vehicles-to-update" },
            new Vehicle() { VehicleId = 19999505, Name = "Vehicles-to-update" },
            new Vehicle() { VehicleId = 19999506, Name = "Vehicles-to-update" },
        };

        public static IEnumerable<Vehicle> GetFilteredVehicles() => GetVehicles.AsQueryable().Where(ListWhere).ToList();

        public static void SeedDatabase(VehiclesContext context)
        {
            var collection = context.Vehicles;

            collection.AddRange(GetVehicles);
            collection.AddRange(VehiclesToDeleteLinqCommandTest);
            collection.AddRange(VehiclesToDeleteLinqRepositoryTest);
            collection.AddRange(VehiclesToDeleteSpecCommandTest);
            collection.AddRange(VehiclesToDeleteSpecRepositoryTest);
            collection.AddRange(VehiclesToUpdate);
            collection.AddRange(OtherVehiclesToUpdate);

            collection.AddRange(VehiclesWithWheels);

            collection.Add(VehicleToDeleteLinqCommandTest);
            collection.Add(VehicleToDeleteInLinqCommandTest2);
            collection.Add(VehicleToDeleteLinqRepositoryTest);
            collection.Add(VehicleToUpdate);
            collection.Add(OtherVehicleToUpdate);

            collection.Add(ToDeleteWithSpecRepository);

            context.SaveChanges();
        }
    }
}
