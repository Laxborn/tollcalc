namespace TollFeeCalculator
{
    // Handles weekends, holidays, day-before-holiday and the July exemption.
    // Personally I'd prefer to use a library for this, but I don't want to assume that's ok, so I made a helper class.
    public static class HolidayProvider
    {
        public static bool IsTollFreeDate(DateTime date)
        {
            // weekends are always free
            if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
                return true;

            // whole of July is free per the requirements
            if (date.Month == 7)
                return true;

            if (IsHoliday(date))
                return true;

            // day before a holiday is also free - just checking tomorrow here
            if (IsHoliday(date.AddDays(1)))
                return true;

            return false;
        }

        // Only the actual named holidays. Kept separate from IsTollFreeDate so July and
        // weekends don't end up leaking into the day-before check above.
        public static bool IsHoliday(DateTime date)
        {
            int day = date.Day;
            int month = date.Month;

            if (month == 1 && day == 1) return true; // New Year's Day
            if (month == 6 && day == 6) return true; // National Day
            if (month == 12 && day == 24) return true; // Christmas Eve
            if (month == 12 && day == 25) return true; // Christmas Day
            if (month == 12 && day == 26) return true; // Boxing Day
            if (month == 12 && day == 31) return true; // New Year's Eve

            return IsMoveableHoliday(date);
        }

        // Easter-based holidays - these move every year so we calculate off Easter Sunday.
        private static bool IsMoveableHoliday(DateTime date)
        {
            DateTime easter = CalculateEasterSunday(date.Year);

            if (date.Date == easter.Date.AddDays(-2)) return true; // Good Friday
            if (date.Date == easter.Date.AddDays(1)) return true; // Easter Monday
            if (date.Date == easter.Date.AddDays(39)) return true; // Ascension Day
            if (date.Date == easter.Date.AddDays(50)) return true; // Whit Monday

            return false;
        }

        // Gauss's Easter algorithm (aka the "Computus"). Looks cryptic but it's the
        // standard formula for this, works for any Gregorian year.
        public static DateTime CalculateEasterSunday(int year)
        {
            int a = year % 19;
            int b = year / 100;
            int c = year % 100;
            int d = b / 4;
            int e = b % 4;
            int f = (b + 8) / 25;
            int g = (b - f + 1) / 3;
            int h = (19 * a + b - d - g + 15) % 30;
            int i = c / 4;
            int k = c % 4;
            int l = (32 + 2 * e + 2 * i - h - k) % 7;
            int m = (a + 11 * h + 22 * l) / 451;
            int month = (h + l - 7 * m + 114) / 31;
            int day = ((h + l - 7 * m + 114) % 31) + 1;

            return new DateTime(year, month, day);
        }
    }
}
