using CleanArchitecture.Domain;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CleanArchitecture.Infrastructure.Persistence.Configurations
{
    public static class Conversions
    {
        public static ValueConverter<string?, string?> Trim => new ValueConverter<string?, string?>(v => v == null ? null : v.TrimEnd().ToUpper(), v => v == null ? null : v.TrimEnd());
        public static ValueConverter<DateTime, int> Date => new ValueConverter<DateTime, int>(v => int.Parse(v.ToString("yyyyMMdd")), v => v.ToDate());

        public static BoolToStringConverter ToBool => new BoolToStringConverter("", "Y");
        
    }
}