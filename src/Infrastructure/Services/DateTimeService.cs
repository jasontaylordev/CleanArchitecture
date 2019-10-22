using CleanArchitecture.Application.Common.Interfaces;
using System;

namespace CleanArchitecture.Infrastructure.Services
{
    public class DateTimeService : IDateTime
    {
        public DateTime Now => DateTime.Now;
    }
}
