namespace TollFeeCalculator
{
    // Handles weekends, holidays, day-before-holiday and the July exemption.
    // Personally I'd prefer to use a library for this, but I don't want to assume that's ok, so I made a helper class.
    public class HolidayProvider : IHolidayProvider
    {
        public bool IsTollFreeDate(DateTime date)
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
        public bool IsHoliday(DateTime date) => (date.Month, date.Day) switch
        {
            (1, 1) => true, // New Year's Day
            (6, 6) => true, // National Day
            (12, 24) => true, // Christmas Eve
            (12, 25) => true, // Christmas Day
            (12, 26) => true, // Boxing Day
            (12, 31) => true, // New Year's Eve
            _ => IsMoveableHoliday(date)
        };

        // Easter-based holidays - these move every year so we calculate off Easter Sunday.
        private static bool IsMoveableHoliday(DateTime date)
        {
            DateTime easter = CalculateEasterSunday(date.Year);
            int[] daysFromEaster = [-2, 1, 39, 50]; // Good Friday, Easter Monday, Ascension Day, Whit Monday

            return daysFromEaster.Any(offset => date.Date == easter.Date.AddDays(offset));
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
