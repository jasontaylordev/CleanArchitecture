using System;
using System.Globalization;

namespace CleanArchitecture.Application.Common.Helpers
{
    public static class PatientHelper
    {
        public static string FormatMyKad(string identification)
        {
            try
            {
                bool result = long.TryParse(identification, out long _);
                if (identification.Length == 12 && result)
                {
                    //double check for IC pattern
                    var yearStr = identification.Substring(0, 2);
                    var monthStr = identification.Substring(2, 2);
                    var dayStr = identification.Substring(4, 2);
                    var now = DateTime.UtcNow;
                    int nowI = int.Parse(now.ToString("yy"));
                    if (int.TryParse(yearStr, out int year))
                    {
                        if (year > nowI && year <= 99)
                        {
                            yearStr = "19" + yearStr;
                        }
                        else
                        {
                            yearStr = "20" + yearStr;
                        }

                        year = int.Parse(yearStr);
                    }

                    var validYear = year > 0 && year <= now.Year;
                    var validMonth = int.TryParse(monthStr, out int month) ? month >= 1 && month <= 12 : false;
                    var validDay = int.TryParse(dayStr, out int day) ? day >= 1 && day <= 31 : false;
                    if (validDay && validMonth && validYear)
                    {
                        return $"{identification.Substring(0, 6)}-{identification.Substring(6, 2)}-{identification.Substring(8, 4)}";
                    }
                }
            }
            catch (Exception)
            {
            }
            return identification;
        }

        public static string ClearMyKadDash(string idenfication)
        {
            return idenfication.Replace("-", "");
        }

        public static DateTime FormatDOBTimezone(DateTime dob)
        {
            //Regardless of sending yyyymmdd 00:00 or yyyymmdd 08:00, always return yyyymmdd 08:00
            DateTime s = dob.Date;
            TimeSpan ts = new TimeSpan(8, 0, 0);
            return s.Date + ts;
        }

        public static int CalculateAge(DateTime dob)
        {
            try
            {
                var now = DateTime.UtcNow;
                if (dob.Date < now.Date)
                {
                    return new DateTime(now.Subtract(dob).Ticks).Year - 1;
                }
            }
            catch (Exception)
            {
            }
            return 0;
        }

        public static DateTime? CalculateDOBFromMyKad(string identification, bool nullable = false)
        {
            try
            {
                if (identification.Length == 14 || identification.Length == 12)
                {
                    CultureInfo enUS = new CultureInfo("en-US");
                    var datetime = DateTime.TryParseExact($"{identification.Substring(0, 6) }", "yyMMdd", enUS,
                                     DateTimeStyles.None, out DateTime dateValue);
                    return dateValue;
                }
                else
                {
                    throw new Exception();
                }
            }
            catch
            {
                if (nullable)
                {
                    return null;
                }
                else
                {
                    return DateTime.UtcNow;
                }
            }
        }
    }
}
