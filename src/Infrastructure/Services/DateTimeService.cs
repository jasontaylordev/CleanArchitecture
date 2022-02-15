using CleanArchitecture.Application.Common.Interfaces;

namespace CleanArchitecture.Infrastructure.Services;

public class DateTimeService : IDateTime
{
    public DateTime Now => DateTime.Now;
}
