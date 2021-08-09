// <copyright file="SortVehicleSpec.cs" company="TryCatch Software Factory">
// Copyright © TryCatch Software Factory All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>

namespace TryCatch.EntityFrameworkCore.UnitTests.Mocks.Spec
{
    using System;
    using System.Linq.Expressions;
    using TryCatch.EntityFrameworkCore.UnitTests.Mocks.Models;
    using TryCatch.Patterns.Specifications;

    public class SortVehicleSpec : ISortSpecification<Vehicle>
    {
        private const string DefaultField = "Name";

        private readonly bool sortAsAscending;

        private readonly string fieldName;

        public SortVehicleSpec(bool asAscending = false, string fieldName = DefaultField)
        {
            if (string.IsNullOrWhiteSpace(fieldName))
            {
                fieldName = DefaultField;
            }

            this.sortAsAscending = asAscending;

            this.fieldName = fieldName.ToUpperInvariant();
        }

        public Expression<Func<Vehicle, object>> AsExpression() =>
            this.fieldName switch
                {
                    "ID" => (x) => x.VehicleId,
                    _ => (x) => x.Name,
                };

        public bool IsAscending() => this.sortAsAscending;
    }
}
