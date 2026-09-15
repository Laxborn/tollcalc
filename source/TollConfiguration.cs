namespace TollFeeCalculator
{
    // Fee schedule and the constants used when calculating a day's total toll.
    public static class TollConfiguration
    {
        public const int MaxDailyFee = 60;
        public const int SameHourWindowMinutes = 60;

        // Hard coded fee schedule based on the given time intervals - simplest way to keep this readable.
        public static int GetFeeByTime(int hour, int minute)
        {
            if (hour == 6 && minute <= 29) return 8;
            if (hour == 6) return 13; // hour == 6 && minute >= 30
            if (hour == 7) return 18;
            if (hour == 8 && minute <= 29) return 13;
            if (hour == 8) return 8; // hour == 8 && minute >= 30
            if (hour > 8 && hour <= 14) return 8;
            if (hour == 15 && minute <= 29) return 13;
            if (hour == 15) return 18; // hour == 15 && minute >= 30
            if (hour == 16) return 18;
            if (hour == 17) return 13;
            if (hour == 18 && minute <= 29) return 8;

            return 0; // everything else (18:30-05:59) is toll-free
        }
    }
}
