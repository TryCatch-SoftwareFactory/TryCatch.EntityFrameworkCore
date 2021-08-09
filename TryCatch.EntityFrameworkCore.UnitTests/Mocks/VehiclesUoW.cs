// <copyright file="VehiclesUoW.cs" company="TryCatch Software Factory">
// Copyright © TryCatch Software Factory All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>

namespace TryCatch.EntityFrameworkCore.UnitTests.Mocks
{
    using TryCatch.EntityFrameworkCore.UnitTests.Mocks.Models;

    public class VehiclesUoW : UnitOfWork
    {
        public VehiclesUoW(VehiclesContext context)
            : base(context)
        {
        }
    }
}
