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

        /// <summary>
        /// Calculates the total toll fee for one day of passages.
        /// </summary>
        /// <param name="vehicle">The vehicle that passed.</param>
        /// <param name="dates">Date and time of every passage, all on the same day. Order doesn't matter.</param>
        /// <returns>The total fee for that day, capped at the daily max.</returns>
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
                if (minutes > TollFeeSchedule.SameHourWindowMinutes)
                {
                    totalFee += windowHighestFee;
                    windowStart = date;
                    windowHighestFee = 0;
                }

                windowHighestFee = Math.Max(windowHighestFee, GetTollFee(vehicle, date));
            }
            totalFee += windowHighestFee;

            return Math.Min(totalFee, TollFeeSchedule.MaxDailyFee);
        }

        /// <summary>
        /// Calculates the toll fee for a single passage.
        /// </summary>
        /// <param name="vehicle">The vehicle that passed.</param>
        /// <param name="date">Date and time of the passage.</param>
        /// <returns>The fee for that passage, or 0 if the day or vehicle is toll-free.</returns>
        public int GetTollFee(IVehicle vehicle, DateTime date)
        {
            ArgumentNullException.ThrowIfNull(vehicle);
            if (holidayProvider.IsTollFreeDate(date) || vehicle.IsTollFree) return 0;

            return TollFeeSchedule.GetFeeByTime(date.Hour, date.Minute);
        }
    }
}
