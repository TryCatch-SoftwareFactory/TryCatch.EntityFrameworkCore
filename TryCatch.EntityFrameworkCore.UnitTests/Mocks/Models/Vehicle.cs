// <copyright file="Vehicle.cs" company="TryCatch Software Factory">
// Copyright © TryCatch Software Factory All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>

namespace TryCatch.EntityFrameworkCore.UnitTests.Mocks.Models
{
    using System;
    using System.Collections.Generic;

    public class Vehicle
    {
        public Vehicle()
        {
            this.Wheels = new List<Wheel>();
        }

        public int VehicleId { get; set; }

        public string Name { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public List<Wheel> Wheels { get; set; }
    }
}
