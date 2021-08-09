// <copyright file="ReadVehicleSpec.cs" company="TryCatch Software Factory">
// Copyright © TryCatch Software Factory All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>

namespace TryCatch.EntityFrameworkCore.UnitTests.Mocks.Spec
{
    using System;
    using System.Linq.Expressions;
    using TryCatch.EntityFrameworkCore.UnitTests.Mocks.Models;
    using TryCatch.Patterns.Specifications.Linq;
    using TryCatch.Validators;

    public class ReadVehicleSpec : CompositeSpecification<Vehicle>, ILinqSpecification<Vehicle>
    {
        private readonly string name;

        public ReadVehicleSpec(string name)
        {
            ArgumentsValidator.ThrowIfIsNullEmptyOrWhiteSpace(name);

            this.name = name;
        }

        public override Expression<Func<Vehicle, bool>> AsExpression() => (x) => x.Name == this.name;
    }
}
