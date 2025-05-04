// <copyright file="DateTimeService.cs" company="Joorak Rezapour">
// Copyright (c) Joorak Rezapour. All rights reserved.
// </copyright>

namespace Infrastructure.Services
{
    /// <summary>
    /// An implementation of <see cref="IDateTimeService"/>.
    /// </summary>
    public class DateTimeService : IDateTimeService
    {
        /// <summary>
        /// Gets the current time.
        /// </summary>
        public DateTime Now => DateTime.Now;
    }
}
