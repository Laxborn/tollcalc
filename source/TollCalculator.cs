namespace TollFeeCalculator
{
    public class TollCalculator : ITollCalculator
    {
        private readonly IHolidayProvider holidayProvider;

        public TollCalculator() : this(new HolidayProvider()) { }

        public TollCalculator(IHolidayProvider holidayProvider)
        {
            this.holidayProvider = holidayProvider;
        }

        /**
         * Calculate the total toll fee for one day
         *
         * @param vehicle - the vehicle
         * @param dates   - date and time of all passes on one day
         * @return - the total toll fee for that day
         */

        public int GetTollFee(IVehicle vehicle, DateTime[] dates)
        {
            ArgumentNullException.ThrowIfNull(vehicle);
            if (dates == null || dates.Length == 0)
                throw new ArgumentException("Dates cannot be null or empty", nameof(dates));

            // order of the passages shouldn't matter, so just sort them first
            DateTime[] passages = dates.OrderBy(d => d).ToArray();

            // the daily cap only works if everything is on the same day
            if (passages[0].Date != passages.Last().Date)
                throw new ArgumentException("All passages must be on the same day", nameof(dates));

            DateTime windowStart = passages[0];
            int windowHighestFee = 0;
            int totalFee = 0;
            foreach (DateTime date in passages)
            {
                long diffInTicks = date.Ticks - windowStart.Ticks;
                long minutes = diffInTicks / TimeSpan.TicksPerMinute;

                // outside the window - charge what we had and start a new one from here
                if (minutes > TollConfiguration.SameHourWindowMinutes)
                {
                    totalFee += windowHighestFee;
                    windowStart = date;
                    windowHighestFee = 0;
                }

                windowHighestFee = Math.Max(windowHighestFee, GetTollFee(date, vehicle));
            }
            totalFee += windowHighestFee;

            return Math.Min(totalFee, TollConfiguration.MaxDailyFee);
        }

        public int GetTollFee(DateTime date, IVehicle vehicle)
        {
            ArgumentNullException.ThrowIfNull(vehicle);
            if (holidayProvider.IsTollFreeDate(date) || vehicle.IsTollFree) return 0;

            return TollConfiguration.GetFeeByTime(date.Hour, date.Minute);
        }
    }
}
