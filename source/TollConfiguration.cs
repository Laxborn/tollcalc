namespace TollFeeCalculator
{
    // Fee schedule and the constants used when calculating a day's total toll.
    public static class TollConfiguration
    {
        public const int MaxDailyFee = 60;
        public const int SameHourWindowMinutes = 60;

        // Hard coded fee schedule based on the given time intervals - simplest way to keep this readable.
        public static int GetFeeByTime(int hour, int minute) => (hour, minute) switch
        {
            (6, >= 0 and <= 29) => 8,
            (6, >= 30 and <= 59) => 13,
            (7, _) => 18,
            (8, >= 0 and <= 29) => 13,
            (8, >= 30 and <= 59) => 8,
            (>= 9 and <= 14, _) => 8,
            (15, >= 0 and <= 29) => 13,
            (15, >= 30 and <= 59) => 18,
            (16, _) => 18,
            (17, _) => 13,
            (18, >= 0 and <= 29) => 8,
            _ => 0 // everything else (18:30-05:59) is toll-free
        };
    }
}
