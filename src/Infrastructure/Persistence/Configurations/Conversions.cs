using System;
using System.Globalization;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CleanArchitecture.Infrastructure.Persistence.Configurations
{
    public static class Conversions
    {
        public static ValueConverter<string?, string?> Trim => new ValueConverter<string?, string?>(v => v == null ? null : v.TrimEnd().ToUpper(), v => v == null ? null : v.TrimEnd());
        public static ValueConverter<DateTime, int> Date => new ValueConverter<DateTime, int>(v => int.Parse(v.ToString("yyyyMMdd")), v => ToDate(v));

        public static BoolToStringConverter ToBool => new BoolToStringConverter("", "Y");
        private static DateTime ToDate(int iseriesDate)
        {
            DateTime dt;
            if (DateTime.TryParseExact(iseriesDate.ToString(), "yyyyMMdd",
                                      CultureInfo.InvariantCulture,
                                      DateTimeStyles.None, out dt))
                return dt;

            return DateTime.MinValue;
        }
    }
}