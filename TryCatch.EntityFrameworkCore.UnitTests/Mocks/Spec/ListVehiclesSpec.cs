// <copyright file="ListVehiclesSpec.cs" company="TryCatch Software Factory">
// Copyright © TryCatch Software Factory All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>

namespace TryCatch.EntityFrameworkCore.UnitTests.Mocks.Spec
{
    using System;
    using System.Linq.Expressions;
    using TryCatch.EntityFrameworkCore.UnitTests.Mocks.Models;
    using TryCatch.Patterns.Specifications.Linq;

    public class ListVehiclesSpec : CompositeSpecification<Vehicle>, ILinqSpecification<Vehicle>
    {
        private readonly string nameCriteria;

        public ListVehiclesSpec(string nameCriteria)
        {
            if (string.IsNullOrWhiteSpace(nameCriteria))
            {
                nameCriteria = string.Empty;
            }

            this.nameCriteria = nameCriteria;
        }

        public override Expression<Func<Vehicle, bool>> AsExpression() => (x) => x.Name.Contains(this.nameCriteria);
    }
}
