// <copyright file="EntitiesFactory.cs" company="TryCatch Software Factory">
// Copyright © TryCatch Software Factory All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>

namespace TryCatch.EntityFrameworkCore.UnitTests.Mocks
{
    using System.Collections.Generic;
    using AutoFixture;
    using TryCatch.EntityFrameworkCore.UnitTests.Mocks.Models;

    public static class EntitiesFactory
    {
        public static T Get<T>()
            where T : class
        {
            var fixture = new Fixture();

            return fixture.Build<T>().Create();
        }

        public static Vehicle Get()
        {
            var fixture = new Fixture();

            var vehicle = fixture.Build<Vehicle>()
                .Without(x => x.VehicleId)
                .Create();

            foreach (var wheel in vehicle.Wheels)
            {
                wheel.WheelId = 0;
            }

            return vehicle;
        }

        public static IEnumerable<T> Get<T>(int numberOfVehicles)
            where T : class
        {
            var fixture = new Fixture();
            var list = new HashSet<T>();

            for (var i = 0; i < numberOfVehicles; i++)
            {
                var item = fixture.Build<T>().Create();

                list.Add(item);
            }

            return list;
        }

        public static IEnumerable<Vehicle> Get(int numberOfVehicles)
        {
            var list = new HashSet<Vehicle>();

            for (var i = 0; i < numberOfVehicles; i++)
            {
                list.Add(Get());
            }

            return list;
        }
    }
}
